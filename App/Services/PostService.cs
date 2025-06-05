using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Enums;

namespace MinTwitterApp.Services;

public class PostService
{
    private readonly ApplicationDbContext _db;

    public PostService(ApplicationDbContext db)
    {
        _db = db;
    }

    public (PostErrorCode ErrorCode, Post? Post) CreatePost(int userId, string content)
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

        return (PostErrorCode.None, post);
    }

    public List<Post> GetAllPosts()
    {
        return _db.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }

    public List<Post> GetPostById(int userId)
    {
        return _db.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }
}
