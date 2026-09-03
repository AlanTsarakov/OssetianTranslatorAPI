using Microsoft.AspNetCore.Mvc;
using OssetianTranslatorAPI.DTOs;
using OssetianTranslatorAPI.Interfaces;

namespace OssetianTranslatorAPI.Controllers;

[ApiController]
[Route("api")]
public class TranslationController : ControllerBase
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IOpenAiTranslationService _translationService;
    private readonly ILogger<TranslationController> _logger;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public TranslationController(
        IImageProcessingService imageProcessingService,
        IOpenAiTranslationService translationService,
        ILogger<TranslationController> logger)
    {
        _imageProcessingService = imageProcessingService;
        _translationService = translationService;
        _logger = logger;
    }

    [HttpPost("translate-image")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<TranslateImageResponse>> TranslateImage(
        IFormFile image,
        CancellationToken cancellationToken)
    {
        if (image is null || image.Length == 0)
        {
            return BadRequest(new TranslateImageResponse
            {
                Success = false,
                Error = "Изображение не было загружено."
            });
        }

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return BadRequest(new TranslateImageResponse
            {
                Success = false,
                Error = $"Недопустимый формат файла. Разрешены: {string.Join(", ", AllowedExtensions)}"
            });
        }

        _logger.LogInformation(
            "Processing image: {FileName}, Size: {Size} bytes",
            image.FileName,
            image.Length);

        try
        {
            await using var stream = image.OpenReadStream();
            var base64Image = _imageProcessingService.ResizeAndConvertToBase64(stream, 1024);

            var translation = await _translationService.TranslateImageAsync(base64Image, cancellationToken);

            return Ok(new TranslateImageResponse
            {
                Success = true,
                Translation = translation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image translation");
            return StatusCode(500, new TranslateImageResponse
            {
                Success = false,
                Error = "Внутренняя ошибка сервера при обработке изображения."
            });
        }
    }
}