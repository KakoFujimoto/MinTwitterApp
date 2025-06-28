using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Models;
using MinTwitterApp.Tests.Common;


namespace MinTwitterApp.Tests;

public class FollowUser_Tests : IDisposable
{
    private readonly ApplicationDbContext db;
    private readonly DateTimeAccessorForUnitTest dateTimeAccessorForUnitTest;

    public FollowUser_Tests()
    {
        db = TestDbHelper.CreateDbContext();
        dateTimeAccessorForUnitTest = new DateTimeAccessorForUnitTest();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task FollowUser_ToggleFollow_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var userA = User.Create(dateTimeAccessorForUnitTest, "フォロワー", "follwer@test.com", "hashedPassword");
        var userB = User.Create(dateTimeAccessorForUnitTest, "フォロイー", "follwee@test.com", "hashedPassword");

        db.Users.AddRange(userA, userB);
        db.SaveChanges();

        var followService = new FollowUserService(db, dateTimeAccessorForUnitTest);

        var firstResult = await followService.ToggleFollowAsync(userA.Id, userB.Id);
        Assert.True(firstResult.IsFollowing);

        var secondResult = await followService.ToggleFollowAsync(userA.Id, userB.Id);
        Assert.False(secondResult.IsFollowing);

        var thirdResult = await followService.ToggleFollowAsync(userA.Id, userB.Id);
        Assert.True(thirdResult.IsFollowing);

    }

}
