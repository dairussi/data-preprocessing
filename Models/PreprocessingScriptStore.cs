public record PreprocessingScriptStore
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ScriptContent { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<ProcessData> ProcessDatas { get; init; } = new List<ProcessData>();

    private PreprocessingScriptStore() { }

    public static Result<PreprocessingScriptStore> Create(string name, string scriptContent, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<PreprocessingScriptStore>("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(scriptContent))
            return Result.Failure<PreprocessingScriptStore>("Script content cannot be empty");

        return Result.Success(new PreprocessingScriptStore
        {
            Name = name,
            ScriptContent = scriptContent,
            CreatedAt = createdAt
        });
    }
}