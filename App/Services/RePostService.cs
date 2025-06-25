using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;

namespace MinTwitterApp.Services;

public class RePostService
{
    private readonly ApplicationDbContext _db;

    private readonly IDateTimeAccessor _dateTimeAccessor;

    public RePostService(
        ApplicationDbContext db,
        IDateTimeAccessor dateTimeAccessor
        )
    {
        _db = db;
        _dateTimeAccessor = dateTimeAccessor;
    }

    public async Task<(PostErrorCode ErrorCode, RePostDTO? Post)> RePostAsync(
        int userId,
        int originalPostId
        )
    {
        var originalPost = await _db.Posts.FindAsync(originalPostId);
        if (originalPost == null || originalPost.IsDeleted)
        {
            return (PostErrorCode.NotFound, null);
        }

        var newPost = new Post
        {
            RepostSourceId = originalPostId,
            UserId = userId,
            Content = originalPost.Content,
            CreatedAt = _dateTimeAccessor.Now,
            UpdatedAt = null,
            IsDeleted = false,
            ImagePath = originalPost.ImagePath
        };

        _db.Posts.Add(newPost);
        await _db.SaveChangesAsync();

        var sourceUser = await _db.Users.FindAsync(originalPost.UserId);

        var postDto = new RePostDTO
        {
            Id = newPost.Id,
            Content = newPost.Content,
            CreatedAt = newPost.CreatedAt,
            ImagePath = newPost.ImagePath,
            UserId = newPost.UserId,
            UserName = (await _db.Users.FindAsync(newPost.UserId))?.Name ?? "Unknown",
            RePostSourceId = originalPost.Id,
            SourceUserName = originalPost.User.Name,
            SourceContent = originalPost.Content

        };

        return (PostErrorCode.None, postDto);
    }
}