using System.Text.Json;

public interface IPreprocessingScriptService
{
    Task<IResult> AddScriptAsync(string name, string scriptContent);
    Task<IResult> GetScriptAsync();
    Task<IResult> ProcessDataAsync(string scriptName, string input);
    Task<IResult> GetProcessDataById(int processDataId);
}