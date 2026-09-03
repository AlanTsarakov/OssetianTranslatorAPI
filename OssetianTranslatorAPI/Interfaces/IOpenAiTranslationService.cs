namespace OssetianTranslatorAPI.Interfaces;

public interface IOpenAiTranslationService
{
    Task<string> TranslateImageAsync(string base64Image, CancellationToken cancellationToken = default);
}