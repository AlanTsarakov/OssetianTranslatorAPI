namespace OssetianTranslatorAPI.Configuration;

public class OpenAiSettings
{
    public const string SectionName = "OpenAI";
    
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o";
    public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxImageWidth { get; set; } = 1024;
}