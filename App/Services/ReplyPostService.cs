using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;

namespace MinTwitterApp.Services;

public class ReplyPostService
{
    private readonly ApplicationDbContext _db;
    private readonly PostErrorService _postErrorService;

    private readonly IDateTimeAccessor _dateTimeAccessor;

    public ReplyPostService(
        ApplicationDbContext db,
        PostErrorService postErrorService,
        IDateTimeAccessor dateTimeAccessor
    )
    {
        _db = db;
        _postErrorService = postErrorService;
        _dateTimeAccessor = dateTimeAccessor;
    }

    public async Task<(PostErrorCode errorCode, ReplyPostDTO? Post)> ReplyPostAsync(
        int userId,
        int originalPostId,
        string? content,
        IFormFile? imageFile
    )
    {
        // 元投稿の存在確認
        var originalPost = await _db.Posts.FindAsync(originalPostId);
        if (originalPost == null || originalPost.IsDeleted)
        {
            return (PostErrorCode.NotFound, null);
        }

        // Reply投稿のバリデーション
        var contentError = _postErrorService.ValidateContent(content);
        if (contentError != PostErrorCode.None)
        {
            return (contentError, null);
        }

        var imageValidationError = _postErrorService.ValidateImage(imageFile);
        if (imageValidationError != PostErrorCode.None)
        {
            return (imageValidationError, null);
        }

        string? savedImagePath = null;
        if (imageFile != null && imageFile.Length > 0)
        {
            savedImagePath = await _postErrorService.SaveImageAsync(imageFile);
        }

        // Reply投稿の作成
        var replyPost = new Post
        {
            UserId = userId,
            Content = content!,
            CreatedAt = _dateTimeAccessor.Now,
            UpdatedAt = null,
            IsDeleted = false,
            RepostSourceId = null,
            ImagePath = savedImagePath
        };

        replyPost.ReplyToPostId = originalPostId;

        _db.Posts.Add(replyPost);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FindAsync(userId);

        var dto = new ReplyPostDTO
        {
            Id = replyPost.Id,
            Content = replyPost.Content,
            userId = replyPost.UserId,
            Username = user?.Name ?? "Unknown",
            ReplyToPostId = originalPost.Id,
            OriginalContent = originalPost.Content,
            originalUserName = originalPost.User?.Name ?? "Unknown"
        };

        return (PostErrorCode.None, dto);

    }
}