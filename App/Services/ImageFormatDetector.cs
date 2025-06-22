using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;


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

}
