using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OssetianTranslatorAPI.Configuration;
using OssetianTranslatorAPI.Interfaces;

namespace OssetianTranslatorAPI.Services;

public class OpenAiTranslationService : IOpenAiTranslationService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;
    private readonly ILogger<OpenAiTranslationService> _logger;

    private const string SystemPrompt =
        """
        Ты — профессиональный переводчик осетинского языка. 
        Твоя задача — распознать осетинский текст на изображении и перевести его на русский язык.
        
        Правила:
        1. Если на изображении есть осетинский текст — распознай его и переведи на русский.
        2. Верни ответ строго в формате JSON: {"original": "распознанный осетинский текст", "translation": "перевод на русский"}
        3. Если осетинского текста на изображении нет, верни: {"original": "", "translation": "Осетинский текст не обнаружен"}
        4. Не добавляй пояснений и комментариев — только JSON.
        """;

    public OpenAiTranslationService(
        HttpClient httpClient,
        IOptions<OpenAiSettings> settings,
        ILogger<OpenAiTranslationService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> TranslateImageAsync(string base64Image, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            model = _settings.Model,
            messages = new object[]
            {
                new { role = "system", content = SystemPrompt },
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = "Распознай и переведи осетинский текст с этого изображения." },
                        new { type = "image_url", image_url = new { url = base64Image } }
                    }
                }
            },
            max_tokens = 1000
        };

        _logger.LogInformation("Sending request to OpenAI model {Model}", _settings.Model);

        var response = await _httpClient.PostAsJsonAsync(
            _settings.Endpoint,
            requestBody,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        
        var content = json
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        _logger.LogInformation("Received response from OpenAI");

        return content ?? string.Empty;
    }
}