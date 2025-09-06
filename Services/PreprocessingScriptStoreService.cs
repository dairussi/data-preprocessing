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
        var scriptExists = await _dbContext.PreprocessingScriptStores.AnyAsync(s => s.Name == name);

        if (scriptExists)
        {
            return Results.Conflict(new
            {
                message = "Já existe um script com esse nome.",
                attemptedName = name
            });
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Results.BadRequest(new { message = "Nome do script é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(scriptContent))
        {
            return Results.BadRequest(new { message = "Conteúdo do script é obrigatório." });
        }

        var preprocessingScript = new PreprocessingScriptStore(name, scriptContent);
        _dbContext.PreprocessingScriptStores.Add(preprocessingScript);
        await _dbContext.SaveChangesAsync();

        return Results.Ok(preprocessingScript);
    }

    public async Task<IResult> GetScriptAsync()
    {
        var scripts = await _dbContext.PreprocessingScriptStores.ToListAsync();
        return Results.Ok(scripts);
    }

    public async Task<IResult> ProcessDataAsync(string scriptName, string input)
    {

        var script = await _dbContext.PreprocessingScriptStores
            .FirstOrDefaultAsync(s => s.Name == scriptName);

        if (script == null)
        {
            return Results.NotFound(new
            {
                message = "Script não encontrado.",
                attemptedName = scriptName
            });
        }

        string[] bannedPatterns = { @"require\s*\(", @"fs\b", @"eval\s*\(", @"import\b", @"global\b" };

        if (bannedPatterns.Any(pattern => Regex.IsMatch(script.ScriptContent, pattern, RegexOptions.IgnoreCase)))
        {
            return Results.BadRequest(new
            {
                message = "O script contém chamadas ou imports não permitidos."
            });
        }

        var processData = new ProcessData(
        preprocessingScriptStoreId: script.Id,
        statusProcess: StatusProcess.InProgress
    );

        _dbContext.ProcessDatas.Add(processData);
        await _dbContext.SaveChangesAsync();

        RunIsolatedProcessing(input, script, processData);

        return Results.Accepted(null, new
        {
            processId = processData.Id,
            status = processData.StatusProcess.ToString(),
            message = "Processamento iniciado com sucesso.",
            createdAt = processData.CreatedAt
        });
    }

    private void RunIsolatedProcessing(string input, PreprocessingScriptStore script, ProcessData processData)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var scopedDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                using var engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);

                engine.Execute(script.ScriptContent);
                string safeJson = JsonSerializer.Serialize(input);

                engine.Execute($"var inputData = JSON.parse({safeJson});");
                var result = engine.Script.process(engine.Script.inputData);

                string jsonResult = engine.Script.JSON.stringify(result);

                var processDataEntity = await scopedDbContext.ProcessDatas.FindAsync(processData.Id);
                if (processDataEntity != null)
                {
                    processDataEntity.AddDataResult(jsonResult);
                    processDataEntity.ModifyProcessing(StatusProcess.Completed);
                    await scopedDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                using var scope = _serviceProvider.CreateScope();
                var scopedDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var processDataEntity = await scopedDbContext.ProcessDatas.FindAsync(processData.Id);
                if (processDataEntity != null)
                {
                    processData.AddErrorMessage(ex.Message);
                    processDataEntity.ModifyProcessing(StatusProcess.Completed);
                    await scopedDbContext.SaveChangesAsync();
                }
            }
        });
    }

    public async Task<IResult> GetProcessDataById(int processDataId)
    {
        var processData = await _dbContext.ProcessDatas
       .Include(pd => pd.PreprocessingScriptStore)
       .FirstOrDefaultAsync(pd => pd.Id == processDataId);

        if (processData == null)
        {
            return Results.NotFound(new
            {
                message = "Processamento não encontrado.",
                attemptedId = processDataId
            });
        }

        return Results.Ok(new
        {
            processId = processData.Id,
            status = processData.StatusProcess.ToString(),
            data = processData.Data,
            scriptName = processData.PreprocessingScriptStore.Name,
            createdAt = processData.CreatedAt
        });
    }

}