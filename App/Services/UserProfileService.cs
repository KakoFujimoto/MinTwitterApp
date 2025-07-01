using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Common;

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
                FollowerCount = _db.Follows.Count(f => f.FolloweeId == u.Id),
                FollowingCount = _db.Follows.Count(f => f.FollowerId == u.Id)
            })
            .FirstOrDefaultAsync();
        return userInfo;
    }

    // 該当ユーザーの投稿一覧を取得する
    public async Task<List<Post>> GetPostByUserAsync(int userId)
    {
        return await _db.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
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