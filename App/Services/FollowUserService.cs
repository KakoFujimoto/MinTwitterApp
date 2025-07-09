using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Common;

namespace MinTwitterApp.Services;

public class FollowUserService
{
    private readonly ApplicationDbContext _db;
    private readonly IDateTimeAccessor _dateTimeAccessor;

    public FollowUserService(ApplicationDbContext db, IDateTimeAccessor dateTimeAccessor)
    {
        _db = db;
        _dateTimeAccessor = dateTimeAccessor;
    }

    // フォロー処理
    public async Task<FollowResultDTO> ToggleFollowAsync(int followerId, int followeeId)
    {
        // 自分自身をフォローしようとした場合は無効
        if (followerId == followeeId)
        {
            return new FollowResultDTO
            {
                IsFollowing = false,
                ErrorCode = PostErrorCode.InvalidOperation
            };
        }

        // 対象ユーザーが存在するか確認
        var followeeExists = await _db.Users.AnyAsync(u => u.Id == followeeId);
        var followerExists = await _db.Users.AnyAsync(u => u.Id == followerId);
        if (!followeeExists || !followerExists)
        {
            return new FollowResultDTO
            {
                IsFollowing = false,
                ErrorCode = PostErrorCode.NotFound
            };
        }

        // 既にフォロー済の場合はフォローを解除する
        var existingFollow = await _db.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

        if (existingFollow != null)
        {
            _db.Follows.Remove(existingFollow);
            await _db.SaveChangesAsync();

            return new FollowResultDTO
            {
                IsFollowing = false,
                ErrorCode = PostErrorCode.None
            };
        }
        else
        {
            var follow = new Follow
            {
                FollowerId = followerId,
                FolloweeId = followeeId,
                CreatedAt = _dateTimeAccessor.Now
            };
            _db.Follows.Add(follow);
            await _db.SaveChangesAsync();

            return new FollowResultDTO
            {
                IsFollowing = true,
                ErrorCode = PostErrorCode.None
            };
        }
    }

    // 自分(followerId)が対象ユーザー(followeeId)をフォローしているかどうかを返す
    public async Task<bool> IsFollowingAsync(int followerId, int followeeId)
    {
        return await _db.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
    }

    // ユーザーがフォローしている人の一覧を返す
    public async Task<List<FollowRelationshipDTO>> GetFollowingUserAsync(int userId)
    {
        return await _db.Follows
            .Where(f => f.FollowerId == userId)
            .Select(f => new FollowRelationshipDTO
            {
                UserId = f.Followee.Id,
                Name = f.Followee.Name
            })
            .ToListAsync();
    }

    // ユーザーがフォローされている人の一覧を返す
    public async Task<List<FollowRelationshipDTO>> GetFollowerUserAsync(int userId)
    {
        return await _db.Follows
            .Where(f => f.FolloweeId == userId)
            .Select(f => new FollowRelationshipDTO
            {
                UserId = f.FollowerId,
                Name = f.Follower.Name,
            })
            .ToListAsync();
    }

    // フォローバック処理
    public async Task<FollowResultDTO> FollowBackAsync(int currentUserId, int targetUserId)
    {
        var isFollowBackTarget = await _db.Follows
            .AnyAsync(f => f.FollowerId == targetUserId && f.FolloweeId == currentUserId);

        if (!isFollowBackTarget)
        {
            return new FollowResultDTO
            {
                IsFollowing = false,
                ErrorCode = PostErrorCode.InvalidOperation
            };
        }

        var alreadyFollowing = await _db.Follows
            .AnyAsync(f => f.FolloweeId == targetUserId && f.FollowerId == currentUserId);

        if (alreadyFollowing)
        {
            return new FollowResultDTO
            {
                IsFollowing = true,
                ErrorCode = PostErrorCode.None
            };
        }

        var follow = new Follow
        {
            FollowerId = currentUserId,
            FolloweeId = targetUserId,
            CreatedAt = _dateTimeAccessor.Now
        };
        _db.Follows.Add(follow);
        await _db.SaveChangesAsync();

        return new FollowResultDTO
        {
            IsFollowing = true,
            ErrorCode = PostErrorCode.None
        };

    }
}
