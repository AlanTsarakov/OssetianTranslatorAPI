namespace OssetianTranslatorAPI.Interfaces;

public interface IImageProcessingService
{
    string ResizeAndConvertToBase64(Stream imageStream, int maxWidth);
}