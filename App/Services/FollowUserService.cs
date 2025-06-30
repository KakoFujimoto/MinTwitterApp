using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.DTO;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

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
        var followwExists = await _db.Users.AnyAsync(u => u.Id == followeeId);
        if (!followwExists)
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

    // フォロワーを取得する
    public async Task<List<User>> GetFollowersAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Followers)
            .ThenInclude(f => f.Follower)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new List<User>();
        }

        return user.Followers.Select(f => f.Follower).ToList();
    }

    // フォロー中のユーザーを取得する
    public async Task<List<User>> GetFollowingUserAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Following)
            .ThenInclude(f => f.Followee)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new List<User>();
        }

        return user.Following.Select(f => f.Followee).ToList();
    }
}
