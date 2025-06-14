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

    public async Task<List<PostPageDTO>> GetAllPostsAsync()
    {
        return await _db.Posts
            .Where(p => !p.IsDeleted)
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
