using MinTwitterApp.Enums;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;

namespace MinTwitterApp.Services;

public class PostErrorService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];

    private const int MaxContentLength = 280;
    private readonly ImageFormatDetector _detector;

    public PostErrorService(ImageFormatDetector detector)
    {
        _detector = detector;
    }

    public PostErrorCode ValidateContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return PostErrorCode.ContentEmpty;
        }
        if (content.Length > MaxContentLength)
        {
            return PostErrorCode.ContentTooLong;
        }
        return PostErrorCode.None;
    }

    public PostErrorCode ValidateImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return PostErrorCode.None;


        var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            return PostErrorCode.InvalidImageExtension;
        }

        var format = _detector.DetectFormat(imageFile);
        if (!IsSupportedFormat(format))
            return PostErrorCode.InvalidImageFormat;

        return PostErrorCode.None;
    }

    public bool IsSupportedFormat(IImageFormat? format)
    {
        return format is JpegFormat or PngFormat or GifFormat;
    }

    public async Task<string?> SaveImageAsync(IFormFile imageFile)
    {
        var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + ext;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using var imageStream = imageFile.OpenReadStream();
        using var output = new FileStream(filePath, FileMode.Create);
        await imageStream.CopyToAsync(output);

        return "/uploads/" + uniqueFileName;
    }
}
