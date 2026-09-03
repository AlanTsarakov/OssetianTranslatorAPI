using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using OssetianTranslatorAPI.Interfaces;

namespace OssetianTranslatorAPI.Services;

public class ImageProcessingService : IImageProcessingService
{
    public string ResizeAndConvertToBase64(Stream imageStream, int maxWidth)
    {
        using var image = Image.Load(imageStream);

        if (image.Width > maxWidth)
        {
            var ratio = (double)maxWidth / image.Width;
            var newHeight = (int)(image.Height * ratio);
            image.Mutate(x => x.Resize(maxWidth, newHeight));
        }

        using var outputStream = new MemoryStream();
        image.SaveAsJpeg(outputStream, new JpegEncoder { Quality = 85 });
        
        var base64 = Convert.ToBase64String(outputStream.ToArray());
        return $"data:image/jpeg;base64,{base64}";
    }
}