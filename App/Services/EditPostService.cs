using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class EditPostService
{
    private readonly ApplicationDbContext _db;

    public EditPostService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PostErrorCode> EditAsync(int postId, string newContent)
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

        post.Content = newContent;
        post.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return PostErrorCode.None;

    }
}