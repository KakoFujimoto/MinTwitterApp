
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;

namespace MinTwitterApp.Services;

public class ImageFormatDetector
{
    public IImageFormat? DetectFormat(IFormFile imageFile)
    {
        try
        {
            using var stream = imageFile.OpenReadStream();
            return Image.DetectFormat(stream);
        }
        catch
        {
            return null;
        }
    }

    public bool IsSupportedFormat(IImageFormat? format)
    {
        return format is JpegFormat or PngFormat or GifFormat;
    }
}
