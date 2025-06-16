using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class DeletePostService
{
    private readonly ApplicationDbContext _db;

    public DeletePostService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PostErrorCode> DeleteAsync(int postId)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
        {
            return PostErrorCode.NotFound;
        }
        
        post.IsDeleted = true;
        await _db.SaveChangesAsync();

        return PostErrorCode.None;
    }
}