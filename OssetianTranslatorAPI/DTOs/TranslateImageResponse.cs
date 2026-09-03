namespace OssetianTranslatorAPI.DTOs;

public class TranslateImageResponse
{
    public bool Success { get; set; }
    public string? Translation { get; set; }
    public string? Error { get; set; }
}