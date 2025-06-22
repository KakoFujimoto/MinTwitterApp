using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class RePostService
{
    private readonly ApplicationDbContext _db;
    private readonly PostErrorService _postErrorService;

    public RePostService(ApplicationDbContext db, PostErrorService postErrorService)
    {
        _db = db;
        _postErrorService = postErrorService;
    }

    public async Task<(PostErrorCode ErrorCode, PostPageDTO? Post)> RePostAsync(Guid userId,
    Guid originalPostId)
    {
        var original = await _db.Posts.FindAsync(originalPostId);
        if (original == null || original.IsDeleted)
        {
            return (PostErrorCode.NotFound, null);
        }

        var repost = Post.Create{

        }
    }
}