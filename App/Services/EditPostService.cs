using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.Enums;

namespace MinTwitterApp.Services;

public class EditPostService
{
    private readonly ApplicationDbContext _db;

    public EditPostService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PostErrorCode> EditAsync(int postId, string newContent, IFormFile? newImageFile, bool deleteImage)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null) return PostErrorCode.NotFound;
        if (post.IsDeleted) return PostErrorCode.AlreadyDeleted;

        post.Content = newContent;
        post.UpdatedAt = DateTime.UtcNow;

        // 画像削除処理
        if (deleteImage && !string.IsNullOrEmpty(post.ImagePath))
        {
            var oldPath = Path.Combine("wwwroot", post.ImagePath.TrimStart('/'));
            if (File.Exists(oldPath)) File.Delete(oldPath);
            post.ImagePath = null;
        }

        // 画像差し替え処理
        if (newImageFile != null && newImageFile.Length > 0)
        {
            // 古い画像を削除（削除済みでなければ）
            if (!string.IsNullOrEmpty(post.ImagePath))
            {
                var oldPath = Path.Combine("wwwroot", post.ImagePath.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(newImageFile.FileName)}";
            var filePath = Path.Combine("wwwroot/uploads", fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await newImageFile.CopyToAsync(stream);

            post.ImagePath = $"/uploads/{fileName}";
        }

        await _db.SaveChangesAsync();
        return PostErrorCode.None;
    }
}
