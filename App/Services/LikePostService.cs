using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;

namespace MinTwitterApp.Services;

public class LikePostService
{
    private readonly ApplicationDbContext _db;

    private readonly PostErrorService _postErrorService;

    public LikePostService(ApplicationDbContext db, PostErrorService postErrorService)
    {
        _db = db;
        _postErrorService = postErrorService;
    }

    public async Task<LikeResultDTO> ToggleLikeAsync(int userId, int postId)
    {
        var post = await _db.Posts.FindAsync(postId);
        if (post == null)
        {
            return new LikeResultDTO
            {
                IsLiked = false,
                LikeCount = 0,
                ErrorCode = PostErrorCode.NotFound
            };
        }
        var existingLike = await _db.Likes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);

        if (existingLike != null)
        {
            _db.Likes.Remove(existingLike);
            await _db.SaveChangesAsync();

            return new LikeResultDTO
            {
                IsLiked = false,
                LikeCount = _db.Likes.Count(l => l.PostId == postId),
                ErrorCode = PostErrorCode.None
            };
        }
        else
        {
            var like = new Like
            {
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.Now
            };
            _db.Likes.Add(like);
            await _db.SaveChangesAsync();
            return new LikeResultDTO
            {
                IsLiked = true,
                LikeCount = _db.Likes.Count(l => l.PostId == postId),
                ErrorCode = PostErrorCode.None
            };
        }
    }

    public int GetLikeCount(int postId)
    {
        return _db.Likes.Count(l => l.PostId == postId);
    }
}
