using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Common;
using System.Collections.Immutable;

namespace MinTwitterApp.Services;

public class UserProfileService
{
    private readonly ApplicationDbContext _db;

    public UserProfileService(ApplicationDbContext db)
    {
        _db = db;
    }

    // プロフィールに表示するユーザーの情報を取得する
    public async Task<UserProfileDTO?> GetUserProfileAsync(int userId)
    {
        var userInfo = await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserProfileDTO
            {
                UserId = u.Id,
                Name = u.Name,
                CreatedAt = u.CreatedAt,
                FollowerCount = u.Followers.Count,
                FollowingCount = u.Following.Count
            })
            .FirstOrDefaultAsync();
        return userInfo;
    }

    // 該当ユーザーの投稿一覧を取得する
    public async Task<List<PostPageDTO>> GetPostDtoByUserAsync(int userId, int currentUserId)
    {
        var posts = await _db.Posts
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .Include(p => p.User)
            .Include(p => p.Replies)
                .ThenInclude(r => r.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // 投稿・返信に含まれるRepostSourceIdを取得
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

        // すべてのリポスト元投稿を取得（ユーザー情報含む）
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
            UserName = p.User?.Name,
            LikeCount = _db.Likes.Count(l => l.PostId == p.Id),
            IsLiked = _db.Likes.Any(l => l.PostId == p.Id && l.UserId == currentUserId),
            RepostSourceId = p.RepostSourceId,
            ReplyToPostId = p.ReplyToPostId,
            SourceUserName = p.RepostSourceId.HasValue
                ? sourcePosts.FirstOrDefault(src => src.Id == p.RepostSourceId.Value)?.User?.Name
                : null,
            SourceContent = p.RepostSourceId.HasValue
                ? sourcePosts.FirstOrDefault(src => src.Id == p.RepostSourceId.Value)?.Content
                : null,
            SourceImagePath = p.RepostSourceId.HasValue
                ? sourcePosts.FirstOrDefault(src => src.Id == p.RepostSourceId.Value)?.ImagePath
                : null,

            Replies = p.Replies
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.CreatedAt)
                .Select(r => new PostPageDTO
                {
                    Id = r.Id,
                    Content = r.Content,
                    ImagePath = r.ImagePath,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId,
                    UserName = r.User?.Name,
                    LikeCount = _db.Likes.Count(l => l.PostId == r.Id),
                    IsLiked = _db.Likes.Any(l => l.PostId == r.Id && l.UserId == currentUserId),
                    RepostSourceId = r.RepostSourceId,
                    ReplyToPostId = r.ReplyToPostId,
                    SourceUserName = r.RepostSourceId.HasValue
                        ? sourcePosts.FirstOrDefault(src => src.Id == r.RepostSourceId.Value)?.User?.Name
                        : null,
                    SourceContent = r.RepostSourceId.HasValue
                        ? sourcePosts.FirstOrDefault(src => src.Id == r.RepostSourceId.Value)?.Content
                        : null,
                    SourceImagePath = r.RepostSourceId.HasValue
                        ? sourcePosts.FirstOrDefault(src => src.Id == r.RepostSourceId.Value)?.ImagePath
                        : null,
                })
                .ToList()
        }).ToList();

        return dtos;
    }



    // フォロワーを取得する
    public async Task<List<User>> GetFollowersAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Followers)
            .ThenInclude(f => f.Follower)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Followers.Select(f => f.Follower).ToList() ?? new List<User>();
    }

    // フォロー中のユーザーを取得する
    public async Task<List<User>> GetFollowingUserAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Following)
            .ThenInclude(f => f.Followee)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Following.Select(f => f.Followee).ToList() ?? new List<User>();
    }

}