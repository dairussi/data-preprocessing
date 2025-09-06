using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Data Preprocessing API",
        Version = "v1",
        Description = "Execute persistência de scripts e pré-processamento de dados.",
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention()
);

builder.Services.AddScoped<IPreprocessingScriptService, PreprocessingScriptService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Data Preprocessing API V1");
    c.RoutePrefix = "swagger";
});

app.MapPost("/preprocessing-script-store", async (string name, string scriptContent, [FromServices] IPreprocessingScriptService service) =>
{
    return await service.AddScriptAsync(name, scriptContent);
});

app.MapGet("/preprocessing-script-store", async ([FromServices] IPreprocessingScriptService service) =>
{
    return await service.GetScriptAsync();
});

app.MapPost("/process-data", async (string scriptName, string input, [FromServices] IPreprocessingScriptService service) =>
{
    return await service.ProcessDataAsync(scriptName, input);
});

app.MapGet("/process-data", async (int processDataId, [FromServices] IPreprocessingScriptService service) =>
{
    return await service.GetProcessDataById(processDataId);
});

app.Run();
