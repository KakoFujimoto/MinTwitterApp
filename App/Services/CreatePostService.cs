using MinTwitterApp.Data;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;

namespace MinTwitterApp.Services;

public class CreatePostService
{
    private readonly ApplicationDbContext _db;
    private readonly PostErrorService _postErrorService;

    public CreatePostService(ApplicationDbContext db, PostErrorService postErrorService)
    {
        _db = db;
        _postErrorService = postErrorService;
    }

    public async Task<(PostErrorCode ErrorCode, PostPageDTO? Post)> CreateAsync(int userId, string content, IFormFile? imageFile)
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
            CreatedAt = DateTime.Now,
            ImagePath = savedImagePath
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        var dto = new PostPageDTO
        {
            Id = post.Id,
            Content = post.Content,
            ImagePath = post.ImagePath,
            UserId = post.UserId,
            CreatedAt = post.CreatedAt
        };

        return (PostErrorCode.None, dto);
    }

}
