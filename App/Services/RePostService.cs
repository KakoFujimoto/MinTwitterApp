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

    public async Task<(PostErrorCode ErrorCode, PostPageDTO? Post)> RePostAsync(
        int userId,
        Guid originalPostId
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

        // RePostDTOに渡すようにする
        var postDto = new PostPageDTO
        {
            Id = newPost.Id,
            Content = newPost.Content,
            CreatedAt = newPost.CreatedAt,
            ImagePath = newPost.ImagePath,
        };

        return (PostErrorCode.None, postDto);
    }
}