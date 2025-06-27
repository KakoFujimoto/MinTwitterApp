using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;
using Microsoft.EntityFrameworkCore;


namespace MinTwitterApp.Services;

public class CreatePostService
{
    private readonly ApplicationDbContext _db;
    private readonly PostErrorService _postErrorService;

    private IDateTimeAccessor _dateTimeAccessor;

    public CreatePostService(
        ApplicationDbContext db,
        PostErrorService postErrorService,
        IDateTimeAccessor dateTimeAccessor)
    {
        _db = db;
        _postErrorService = postErrorService;
        _dateTimeAccessor = dateTimeAccessor;
    }

    public async Task<(PostErrorCode ErrorCode, PostPageDTO? Post)> CreateAsync(
        int userId,
        string content,
        IFormFile? imageFile
        )
    {
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

        var post = new Post
        {
            UserId = userId,
            Content = content,
            CreatedAt = _dateTimeAccessor.Now,
            ImagePath = savedImagePath
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        var savedPost = await _db.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == post.Id);

        if (savedPost == null)
        {
            return (PostErrorCode.NotFound, null);
        }

        var dto = new PostPageDTO
        {
            Id = savedPost.Id,
            Content = savedPost.Content,
            ImagePath = savedPost.ImagePath,
            UserId = savedPost.UserId,
            UserName = savedPost.User.Name,
            CreatedAt = savedPost.CreatedAt
        };

        return (PostErrorCode.None, dto);
    }
}
