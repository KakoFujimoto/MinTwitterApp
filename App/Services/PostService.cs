using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Enums;
using MinTwitterApp.DTO;

namespace MinTwitterApp.Services;

public class PostService
{
    private readonly ApplicationDbContext _db;

    public PostService(ApplicationDbContext db)
    {
        _db = db;
    }

    public (PostErrorCode, PostPageDTO?) CreatePost(int userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return (PostErrorCode.ContentEmpty, null);
        }

        var post = new Post
        {
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.Now
        };

        _db.Posts.Add(post);
        _db.SaveChanges();

        var dto = new PostPageDTO
        {
            Id = post.Id,
            Content = post.Content,
            UserId = post.UserId,
            CreatedAt = post.CreatedAt
        };

        return (PostErrorCode.None, dto);
    }

    public List<PostPageDTO> GetAllPosts()
    {
        return _db.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostPageDTO
            {
                Id = p.Id,
                Content = p.Content,
                ImagePath = p.ImagePath,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId
            })
            .ToList();
    }

    public List<PostPageDTO> GetPostById(int userId)
    {
        return _db.Posts
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
            .ToList();

    }
}
