using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Models;

namespace MinTwitterApp.Services;

public class LikePostService
{
    private readonly ApplicationDbContext db;

    public LikePostService(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<LikeResultDTO> ToggleLikeAsync(int userId, int postId)
    {
        var existingLike = await db.Likes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);

        if (existingLike != null)
        {
            db.Likes.Remove(existingLike);
            await db.SaveChangesAsync();
            return new LikeResultDTO { IsLiked = false };
        }
        else
        {
            var like = new Like
            {
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.Now
            };
            db.Likes.Add(like);
            await db.SaveChangesAsync();
            return new LikeResultDTO { IsLiked = true };
        }
    }
}
