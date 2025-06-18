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
    public async Task<List<PostPageDTO>> GetAllPostsAsync()
    {
        return await _db.Posts
        .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostPageDTO
            {
                Id = p.Id,
                Content = p.Content,
                ImagePath = p.ImagePath,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = p.User.Name
            })
            .ToListAsync();
    }

    public async Task<List<PostPageDTO>> GetPostsByUserIdAsync(int userId)
    {
        return await _db.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostPageDTO
            {
                Id = p.Id,
                Content = p.Content,
                ImagePath = p.ImagePath,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId
            })
            .ToListAsync();
    }
}
