using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class ViewPostService
{
    private readonly ApplicationDbContext _db;

    public ViewPostService(ApplicationDbContext db)
    {
        _db = db;
    }

    // 投稿取得処理にはグローバルクエリフィルターでIsDeletedを表示させていない
    public async Task<List<PostPageDTO>> GetAllPostsAsync(int currentUserId)
    {
        var posts = await _db.Posts
            .Where(p => !p.IsDeleted)
            .Include(p => p.User)
            .Include(p => p.Replies)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var sourceIdList = posts
            .Select(x => x.RepostSourceId)
            .Where(x => x.HasValue)
            .Cast<int>()
            .ToList();

        var sourceList = await _db.Posts
            .Where(x => sourceIdList.Contains(x.Id))
            .ToListAsync();

        var dtos = posts.Select(p => new PostPageDTO
        {
            Id = p.Id,
            Content = p.Content,
            ImagePath = p.ImagePath,
            CreatedAt = p.CreatedAt,
            UserId = p.UserId,
            UserName = p.User.Name,
            LikeCount = _db.Likes.Count(l => l.PostId == p.Id),
            IsLiked = _db.Likes.Any(l => l.PostId == p.Id && l.UserId == currentUserId),
            RepostSourceId = p.RepostSourceId,
            SourceUserId = p.RepostSourceId.HasValue
                ? _db.Posts.Where(src => src.Id == p.RepostSourceId).Select(src => src.User.Id).FirstOrDefault()
                : null,
            SourceUserName = p.RepostSourceId.HasValue
                ? sourceList.Where(rp => rp.Id == p.RepostSourceId.Value)
                    .Select(rp => rp.User.Name).FirstOrDefault()
                : null,
            SourceContent = p.RepostSourceId.HasValue
                ? sourceList.Where(rp => rp.Id == p.RepostSourceId.Value)
                    .Select(rp => rp.Content).FirstOrDefault()
                : null,
            SourceImagePath = p.RepostSourceId.HasValue
                ? sourceList.Where(rp => rp.Id == p.RepostSourceId.Value)
                    .Select(rp => rp.ImagePath).FirstOrDefault()
                : null,
            ReplyToPostId = p.ReplyToPostId,
            Replies = p.Replies
                .OrderBy(r => r.CreatedAt)
                .Select(r => new PostPageDTO
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    UserName = r.User.Name,
                    UserId = r.UserId,
                    LikeCount = _db.Likes.Count(l => l.PostId == r.Id),
                    IsLiked = _db.Likes.Any(l => l.PostId == r.Id && l.UserId == currentUserId),
                    RepostSourceId = r.RepostSourceId,
                    SourceUserName = r.RepostSourceId.HasValue
                        ? _db.Posts.Where(src => src.Id == r.ReplyToPostId).Select(src => src.User.Name).FirstOrDefault()
                        : null,
                    SourceContent = r.RepostSourceId.HasValue
                        ? _db.Posts.Where(src => src.Id == r.RepostSourceId).Select(src => src.Content).FirstOrDefault()
                        : null,
                    SourceImagePath = r.RepostSourceId.HasValue
                        ? _db.Posts.Where(src => src.Id == r.RepostSourceId).Select(src => src.ImagePath).FirstOrDefault()
                        : null,
                    ImagePath = r.ImagePath,
                    ReplyToPostId = r.ReplyToPostId
                }).ToList()

        }).ToList();

        return dtos;
    }


    public async Task<List<PostPageDTO>> GetPostsByUserIdAsync(int userId)
    {
        var posts = await _db.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var dtos = posts.Select(p => new PostPageDTO
        {
            Id = p.Id,
            Content = p.Content,
            ImagePath = p.ImagePath,
            CreatedAt = p.CreatedAt,
            UserId = p.UserId,
            UserName = p.User?.Name,
            LikeCount = _db.Likes.Count(l => l.PostId == p.Id)
        }).ToList();

        return dtos;
    }

}