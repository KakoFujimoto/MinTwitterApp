using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Tests.Common;

namespace MinTwitterApp.Tests;

public class LikeCount_Tests : IDisposable
{
    private readonly ApplicationDbContext db;
    private readonly DateTimeAccessorForUnitTest dateTimeAccessorForUnitTest;

    public LikeCount_Tests()
    {
        db = TestDbHelper.CreateDbContext();
        dateTimeAccessorForUnitTest = new DateTimeAccessorForUnitTest();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task GetLikeCount_ShouldReturnCorrectCount()
    {
        using var transaction = db.Database.BeginTransaction();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);

        var user1 = User.Create(dateTimeAccessorForUnitTest, "ユーザー1", "user1@test.com", "hashedPassword");
        var user2 = User.Create(dateTimeAccessorForUnitTest, "ユーザー2", "user2@test.com", "hashedPassword");
        db.Users.AddRange(user1, user2);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, postErrorService, dateTimeAccessorForUnitTest);
        var (errorCode, postDto) = await createPostService.CreateAsync(user1.Id, "いいねされる投稿", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var likeService = new LikePostService(db, postErrorService);

        await likeService.ToggleLikeAsync(user1.Id, postDto!.Id);
        await likeService.ToggleLikeAsync(user2.Id, postDto.Id);

        var likeCount = likeService.GetLikeCount(postDto.Id);

        Assert.Equal(2, likeCount);

        transaction.Rollback();
    }
}
