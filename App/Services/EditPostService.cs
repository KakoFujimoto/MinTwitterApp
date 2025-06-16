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

        // 本文バリデーション
        var contentError = _errorService.ValidateContent(newContent);
        if (contentError != PostErrorCode.None)
        {
            return contentError;
        }

        post.Content = newContent;
        post.UpdatedAt = DateTime.UtcNow;

        // 画像削除処理
        if (deleteImage && !string.IsNullOrEmpty(post.ImagePath))
        {
            DeleteImageFile(post.ImagePath);
            post.ImagePath = null;
        }

        // 画像差し替え処理
        if (newImageFile != null && newImageFile.Length > 0)
        {
            // 古い画像を削除（削除済みでなければ）
            if (!string.IsNullOrEmpty(post.ImagePath))
            {
                DeleteImageFile(post.ImagePath);
            }

            var (imageError, savedPath) = await _errorService.ValidateAndSaveImageAsync(newImageFile);
            if (imageError != PostErrorCode.None)
            {
                return imageError;
            }

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
