using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.ClearScript.V8;
using Microsoft.EntityFrameworkCore;

public class PreprocessingScriptService : IPreprocessingScriptService
{
    private readonly AppDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public PreprocessingScriptService(AppDbContext dbContext, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public async Task<IResult> AddScriptAsync(string name, string scriptContent)
    {
        if (await ScriptExistsAsync(name))
            return Results.Conflict(new { message = "Já existe um script com esse nome.", attemptedName = name });

        var createdAt = DateTime.UtcNow;
        var scriptResult = await PreprocessingScriptStore.Create(name, scriptContent, createdAt)
            .Match(
                async success =>
                {
                    _dbContext.PreprocessingScriptStores.Add(success);
                    await _dbContext.SaveChangesAsync();
                    return Result.Success(success);
                },
                error => Task.FromResult(Result.Failure<PreprocessingScriptStore>(error))
            );

        return scriptResult.Match(
            success => Results.Ok(success),
            error => Results.BadRequest(new { message = error })
        );
    }

    public async Task<IResult> GetScriptAsync()
    {
        var scripts = await _dbContext.PreprocessingScriptStores.ToListAsync();
        return Results.Ok(scripts);
    }

    public async Task<IResult> ProcessDataAsync(string scriptName, string input)
    {
        var scriptResult = await GetScriptByNameAsync(scriptName);

        return await scriptResult.Match(
            async script =>
            {
                if (ContainsBannedPatterns(script.ScriptContent))
                    return Results.BadRequest(new { message = "O script contém chamadas ou imports não permitidos." });

                var createdAt = DateTime.UtcNow;
                var processId = Guid.NewGuid();

                var processDataResult = await ProcessData.Create(script.Id, processId, createdAt)
                    .Match(
                        async success =>
                        {
                            _dbContext.ProcessDatas.Add(success);
                            await _dbContext.SaveChangesAsync();
                            await ProcessDataInBackground(input, script, success);
                            return Result.Success(success);
                        },
                        error => Task.FromResult(Result.Failure<ProcessData>(error))
                    );

                return processDataResult.Match(
                    success => Results.Accepted(null, new
                    {
                        processId = success.ProcessId,
                        status = success.StatusProcess.ToString(),
                        message = "Processamento iniciado com sucesso.",
                        createdAt = success.CreatedAt
                    }),
                    error => Results.BadRequest(new { message = error })
                );
            },
            error => Task.FromResult(Results.NotFound(new { message = error, attemptedName = scriptName }))
        );
    }


    private async Task ProcessDataInBackground(string input, PreprocessingScriptStore script, ProcessData processData)
    {
        try
        {
            var resultTask = await ExecuteScriptAsync(input, script.ScriptContent);
            await resultTask.Match(
                async success => await UpdateProcessDataAsync(processData.ProcessId, pd => pd.WithData(success)),
                async error => await UpdateProcessDataAsync(processData.ProcessId, pd => pd.WithError(error))
            );
        }
        catch (Exception ex)
        {
            await UpdateProcessDataAsync(processData.ProcessId, pd => pd.WithError(ex.Message));
        }
    }

    private async Task<Result<string>> ExecuteScriptAsync(string input, string scriptContent)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);
                engine.Execute(scriptContent);

                string safeJson = JsonSerializer.Serialize(input);
                engine.Execute($"var inputData = JSON.parse({safeJson});");

                var result = engine.Script.process(engine.Script.inputData);
                return Result.Success(engine.Script.JSON.stringify(result));
            }
            catch (Exception ex)
            {
                return Result.Failure<string>($"Script execution failed: {ex.Message}");
            }
        });
    }

    private async Task<Result<PreprocessingScriptStore>> GetScriptByNameAsync(string name)
    {
        var script = await _dbContext.PreprocessingScriptStores
            .FirstOrDefaultAsync(s => s.Name == name);

        return script != null
            ? Result.Success(script)
            : Result.Failure<PreprocessingScriptStore>("Script não encontrado");
    }

    private async Task<bool> ScriptExistsAsync(string name) =>
        await _dbContext.PreprocessingScriptStores.AnyAsync(s => s.Name == name);

    private static bool ContainsBannedPatterns(string scriptContent)
    {
        string[] bannedPatterns = { @"require\s*\(", @"fs\b", @"eval\s*\(", @"import\b", @"global\b" };
        return bannedPatterns.Any(pattern =>
            Regex.IsMatch(scriptContent, pattern, RegexOptions.IgnoreCase));
    }

    private async Task UpdateProcessDataAsync(Guid processId, Func<ProcessData, Result<ProcessData>> updateFunc)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var processData = await scopedDbContext.ProcessDatas
            .Where(pd => pd.ProcessId == processId)
            .OrderByDescending(pd => pd.CreatedAt)
            .FirstOrDefaultAsync();

        if (processData == null) return;

        var updateResult = updateFunc(processData);
        await updateResult.Match(
            async success =>
            {
                var newProcessData = success with { Id = 0, ProcessId = processData.ProcessId };
                await scopedDbContext.ProcessDatas.AddAsync(newProcessData);
                await scopedDbContext.SaveChangesAsync();
            },
            _ => Task.CompletedTask
        );
    }


    public async Task<IResult> GetProcessDataByProcessId(Guid processId)
    {
        var processDatas = await _dbContext.ProcessDatas
            .Include(pd => pd.PreprocessingScriptStore)
            .Where(pd => pd.ProcessId == processId)
            .OrderBy(pd => pd.CreatedAt)
            .ToListAsync();

        if (!processDatas.Any())
            return Results.NotFound(new { message = "Processamento não encontrado.", attemptedScriptId = processId });

        var result = processDatas.Select(pd => new
        {
            processId = pd.ProcessId,
            status = pd.StatusProcess.ToString(),
            data = pd.Data,
            scriptName = pd.PreprocessingScriptStore.Name,
            createdAt = pd.CreatedAt
        });

        return Results.Ok(result);
    }
}