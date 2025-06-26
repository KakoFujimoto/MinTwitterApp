using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;

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
        string? content
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

        // Reply投稿の作成
        var replyPost = new Post
        {
            UserId = userId,
            Content = content!,
            CreatedAt = _dateTimeAccessor.Now,
            UpdatedAt = null,
            IsDeleted = false,
            RepostSourceId = null
        };

        // ReplyToPostIdをPostプロパティに足す必要がある
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