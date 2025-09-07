public record ProcessData
{
    public int Id { get; init; }               // PK do banco, autogerado
    public Guid ProcessId { get; init; }       // Identidade conceitual, mesma para versões
    public string? Data { get; init; }
    public StatusProcess StatusProcess { get; init; }
    public DateTime CreatedAt { get; init; }
    public int PreprocessingScriptStoreId { get; init; }
    public string? ErrorMessage { get; init; }
    public PreprocessingScriptStore PreprocessingScriptStore { get; init; } = default!;

    private ProcessData() { }

    public static Result<ProcessData> Create(int preprocessingScriptStoreId, Guid processId, DateTime createdAt) =>
        Result.Success(new ProcessData
        {
            CreatedAt = createdAt,
            PreprocessingScriptStoreId = preprocessingScriptStoreId,
            ProcessId = processId,
            StatusProcess = StatusProcess.InProgress
        });

    public Result<ProcessData> WithData(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return Result.Failure<ProcessData>("O resultado do processamento está vazio");

        return Result.Success(this with
        {
            Data = data,
            StatusProcess = StatusProcess.Completed
        });
    }

    public Result<ProcessData> WithError(string errorMessage) =>
        Result.Success(this with
        {
            ErrorMessage = errorMessage,
            StatusProcess = StatusProcess.Failed
        });

    public Result<ProcessData> WithStatus(StatusProcess status) =>
        Result.Success(this with { StatusProcess = status });
}
