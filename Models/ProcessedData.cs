public class ProcessData
{
    public int Id { get; private set; }
    public string? Data { get; private set; }
    public StatusProcess StatusProcess { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int PreprocessingScriptStoreId { get; private set; }
    public string? ErrorMessage { get; private set; }
    public PreprocessingScriptStore PreprocessingScriptStore { get; private set; } = default!;

    public ProcessData() { }

    public ProcessData(int preprocessingScriptStoreId, StatusProcess statusProcess)
    {
        CreatedAt = DateTime.UtcNow;
        PreprocessingScriptStoreId = preprocessingScriptStoreId;
        StatusProcess = statusProcess;
    }

    public void AddDataResult(string data)
    {
        Data = data;
    }

    public void ModifyProcessing(StatusProcess statusProcess)
    {
        StatusProcess = statusProcess;
    }

    public void AddErrorMessage(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

}