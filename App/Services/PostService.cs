using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Enums;
using MinTwitterApp.DTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MinTwitterApp.Services;

public class PostService
{
    private readonly ApplicationDbContext _db;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];

    public PostService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<(PostErrorCode ErrorCode, PostPageDTO? Post)> CreatePostAsync(int userId, string content, IFormFile? imageFile)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return (PostErrorCode.ContentEmpty, null);
        }

        string? savedImagePath = null;

        if (imageFile != null && imageFile.Length > 0)
        {
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
            {
                return (PostErrorCode.InvalidImageExtension, null);
            }

            try
            {
                using var imageStream = imageFile.OpenReadStream();
                IImageFormat? format = Image.DetectFormat(imageStream);

                if (format == null ||
                    (format.Name != "JPEG" && format.Name != "PNG" && format.Name != "GIF"))
                {
                    return (PostErrorCode.InvalidImageFormat, null);
                }

                imageStream.Position = 0;

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var output = new FileStream(filePath, FileMode.Create))
                {
                    imageStream.CopyTo(output);
                }

                savedImagePath = "/uploads/" + uniqueFileName;
            }
            catch (UnknownImageFormatException)
            {
                return (PostErrorCode.InvalidImageFormat, null);
            }
            catch (Exception)
            {
                return (PostErrorCode.ImageReadError, null);
            }
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

    public async Task<List<PostPageDTO>> GetAllPostsAsync()
    {
        return await _db.Posts
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

    public async Task<List<PostPageDTO>> GetPostByUserIdAsync(int userId)
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
