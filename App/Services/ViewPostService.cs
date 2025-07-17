using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class ViewPostService
{
    private readonly ApplicationDbContext _db;
    private readonly FollowUserService _followUserService;

    public ViewPostService(ApplicationDbContext db, FollowUserService followUserService)
    {
        _db = db;
        _followUserService = followUserService;
    }

    // 投稿取得処理にはグローバルクエリフィルターでIsDeletedを表示させていない
    // 共通部分のクエリ
    private IQueryable<Post> BaseQuery => _db.Posts
            .Include(p => p.User)
            .Include(p => p.Replies)
                .ThenInclude(r => r.User);


    // フォローしているユーザーの投稿取得(未使用/拡張用)
    public async Task<List<PostPageDTO>> GetFollowerPostsAsync(int currentUserId, int userId)
    {
        var query = BaseQuery;

        var followers = await _followUserService.GetFollowerUserAsync(userId);
        var followerIds = followers.Select(x => x.UserId).ToList();

        query = query.Where(x => followerIds.Contains(userId));

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return await MapPostsToDTOsAsync(currentUserId, posts);
    }

    // 全投稿取得(タイムライン用)
    public async Task<List<PostPageDTO>> GetPostsAsync(int currentUserId)
    {
        var query = BaseQuery;

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return await MapPostsToDTOsAsync(currentUserId, posts);
    }


    // 特定ユーザーの投稿取得(プロフィール画面用)
    public async Task<List<PostPageDTO>> GetPostsAsync(int currentUserId, int filterUserId)
    {
        var query = BaseQuery;

        query = query.Where(p => p.UserId == filterUserId);

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return await MapPostsToDTOsAsync(currentUserId, posts);
    }

    // - 処理調整必要か
    public async Task<List<PostPageDTO>> MapPostsToDTOsAsync(int currentUserId, List<Post> posts)
    {
        // Repost元取得
        var sourceIds = posts
                .Where(p => p.RepostSourceId.HasValue)
                .Select(p => p.RepostSourceId!.Value)
                .ToList();

        var replySourceIds = posts
            .SelectMany(p => p.Replies)
            .Where(r => r.RepostSourceId.HasValue)
            .Select(r => r.RepostSourceId!.Value)
            .ToList();

        sourceIds.AddRange(replySourceIds);
        sourceIds = sourceIds.Distinct().ToList();

        var sourcePosts = await _db.Posts
            .Where(p => sourceIds.Contains(p.Id))
            .Include(p => p.User)
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
                ? sourcePosts.Where(rp => rp.Id == p.RepostSourceId.Value)
                    .Select(rp => rp.User.Name).FirstOrDefault()
                : null,
            SourceContent = p.RepostSourceId.HasValue
                ? sourcePosts.Where(rp => rp.Id == p.RepostSourceId.Value)
                    .Select(rp => rp.Content).FirstOrDefault()
                : null,
            SourceImagePath = p.RepostSourceId.HasValue
                ? sourcePosts.Where(rp => rp.Id == p.RepostSourceId.Value)
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