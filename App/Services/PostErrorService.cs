using MinTwitterApp.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace MinTwitterApp.Services;

public class PostErrorService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
    private readonly ImageFormatDetector _detector;

    public PostErrorService(ImageFormatDetector detector)
    {
        _detector = detector;
    }

    public PostErrorCode ValidateContent(string? content)
    {
        return string.IsNullOrWhiteSpace(content)
            ? PostErrorCode.ContentEmpty
            : PostErrorCode.None;
    }

    public PostErrorCode ValidateImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return PostErrorCode.None;

        var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return PostErrorCode.InvalidImageExtension;

        var format = _detector.DetectFormat(imageFile);
        if (!_detector.IsSupportedFormat(format))
            return PostErrorCode.InvalidImageFormat;

        return PostErrorCode.None;
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
