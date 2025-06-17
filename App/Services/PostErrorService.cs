using MinTwitterApp.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace MinTwitterApp.Services;

public class PostErrorService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];

    public PostErrorCode ValidateContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return PostErrorCode.ContentEmpty;
        }

        return PostErrorCode.None;
    }


    public PostErrorCode ValidateImage(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return PostErrorCode.None;
        }

        var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            return PostErrorCode.InvalidImageExtension;
        }

        try
        {
            using var imageStream = imageFile.OpenReadStream();
            IImageFormat? format = Image.DetectFormat(imageStream);

            if (format == null || (format.Name != "JPEG" && format.Name != "PNG" && format.Name != "GIF"))
            {
                return PostErrorCode.InvalidImageFormat;
            }
        }
        catch (UnknownImageFormatException)
        {
            return PostErrorCode.InvalidImageFormat;
        }
        catch
        {
            return PostErrorCode.ImageReadError;
        }

        return PostErrorCode.None;
    }

    public async Task<string?> SaveImageAsync(IFormFile imageFile)
    {
        var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + ext;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var imageStream = imageFile.OpenReadStream())
        using (var output = new FileStream(filePath, FileMode.Create))
        {
            await imageStream.CopyToAsync(output);
        }

        return "/uploads/" + uniqueFileName;
    }

}
