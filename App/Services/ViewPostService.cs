using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class ViewPostService
{
    private readonly ApplicationDbContext _db;

    public ViewPostService(ApplicationDbContext db)
    {
        _db = db;
    }

    // 投稿取得処理にはグローバルクエリフィルターでIsDeletedを表示させていない
    public async Task<List<PostPageDTO>> GetAllPostsAsync(int currentUserId)
    {
        var posts = await _db.Posts
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var dtos = posts.Select(p => new PostPageDTO
        {
            Id = p.Id,
            Content = p.Content,
            ImagePath = p.ImagePath,
            CreatedAt = p.CreatedAt,
            UserId = p.UserId,
            UserName = p.User.Name,
            LikeCount = _db.Likes.Count(l => l.PostId == p.Id),
            IsLiked = _db.Likes.Any(l => l.PostId == p.Id && l.UserId == currentUserId)
        }).ToList();

        return dtos;
    }


    public async Task<List<PostPageDTO>> GetPostsByUserIdAsync(int userId)
    {
        var posts = await _db.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var dtos = posts.Select(p => new PostPageDTO
        {
            Id = p.Id,
            Content = p.Content,
            ImagePath = p.ImagePath,
            CreatedAt = p.CreatedAt,
            UserId = p.UserId,
            UserName = p.User?.Name,
            LikeCount = _db.Likes.Count(l => l.PostId == p.Id)
        }).ToList();

        return dtos;
    }

}