using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.Enums;

namespace MinTwitterApp.Services;

public class EditPostService
{
    private readonly ApplicationDbContext _db;
    private readonly PostErrorService _errorService;

    public EditPostService(ApplicationDbContext db, PostErrorService errorService)
    {
        _db = db;
        _errorService = errorService;
    }

    public async Task<PostErrorCode> EditAsync(int postId, string newContent, IFormFile? newImageFile, bool deleteImage)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return PostErrorCode.NotFound;
        }
        if (post.IsDeleted)
        {
            return PostErrorCode.AlreadyDeleted;
        }

        var contentError = _errorService.ValidateContent(newContent);
        if (contentError != PostErrorCode.None)
        {
            return contentError;
        }

        post.Content = newContent;
        post.UpdatedAt = DateTime.UtcNow;

        if (deleteImage && !string.IsNullOrEmpty(post.ImagePath))
        {
            DeleteImageFile(post.ImagePath);
            post.ImagePath = null;
        }

        if (newImageFile != null && newImageFile.Length > 0)
        {
            var imageError = _errorService.ValidateImage(newImageFile);
            if (imageError != PostErrorCode.None)
            {
                return imageError;
            }

            if (!string.IsNullOrEmpty(post.ImagePath))
            {
                DeleteImageFile(post.ImagePath);
            }

            var savedPath = await _errorService.SaveImageAsync(newImageFile);
            post.ImagePath = savedPath;
        }


        await _db.SaveChangesAsync();
        return PostErrorCode.None;
    }

    private void DeleteImageFile(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return;
        }

        var fullPath = Path.Combine("wwwroot", imagePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
