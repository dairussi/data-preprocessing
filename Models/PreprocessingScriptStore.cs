public class PreprocessingScriptStore
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ScriptContent { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }
    public List<ProcessData> ProcessDatas { get; private set; } = new();

    public PreprocessingScriptStore() { }

    public PreprocessingScriptStore(string name, string scriptContent)
    {
        Name = name;
        ScriptContent = scriptContent;
        CreatedAt = DateTime.UtcNow;
    }
}