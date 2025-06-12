using MinTwitterApp.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace MinTwitterApp.Services;

public class PostErrorService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];

    public PostErrorCode ValidateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return PostErrorCode.ContentEmpty;
        }

        return PostErrorCode.None;
    }

    public async Task<(PostErrorCode ErrorCode, string? SavedImagePath)> ValidateAndSaveImageAsync(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return (PostErrorCode.None, null);
        }

        var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            return (PostErrorCode.InvalidImageExtension, null);
        }

        try
        {
            using var imageStream = imageFile.OpenReadStream();
            IImageFormat? format = Image.DetectFormat(imageStream);

            if (format == null || (format.Name != "JPEG" && format.Name != "PNG" && format.Name != "GIF"))
            {
                return (PostErrorCode.InvalidImageFormat, null);
            }

            imageStream.Position = 0;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + ext;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var output = new FileStream(filePath, FileMode.Create))
            {
                await imageStream.CopyToAsync(output);
            }

            var savedPath = "/uploads/" + uniqueFileName;
            return (PostErrorCode.None, savedPath);
        }
        catch (UnknownImageFormatException)
        {
            return (PostErrorCode.InvalidImageFormat, null);
        }
        catch
        {
            return (PostErrorCode.ImageReadError, null);
        }
    }
}
