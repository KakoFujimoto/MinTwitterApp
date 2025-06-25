using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Tests.Common;

namespace MinTwitterApp.Tests;

public class CreatePost_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    public CreatePost_Tests()
    {
        db = TestDbHelper.CreateDbContext();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task CreatePost_Ok_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var user = new User
        {
            Name = "テストユーザー",
            Email = "test@example.com",
            PassWordHash = "dummyhash"
        };

        db.Users.Add(user);
        db.SaveChanges();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);
        var createPostService = new CreatePostService(db, postErrorService);

        var result = await createPostService.CreateAsync(user.Id, "テスト投稿", null);

        Assert.Equal(PostErrorCode.None, result.ErrorCode);
        Assert.NotNull(result.Post);
        Assert.Equal(user.Id, result.Post!.UserId);
        Assert.Equal("テスト投稿", result.Post.Content);

        transaction.Rollback();
    }

    [Fact]
    public async Task CreatePostAsync_Empty_ShoudReturnError()
    {
        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);
        var createPostService = new CreatePostService(db, postErrorService);

        var result = await createPostService.CreateAsync(1, "", null);

        Assert.Equal(PostErrorCode.ContentEmpty, result.ErrorCode);
        Assert.Null(result.Post);
    }

    [Theory]
    [InlineData(279, PostErrorCode.None)]
    [InlineData(280, PostErrorCode.None)]
    [InlineData(281, PostErrorCode.ContentTooLong)]
    [InlineData(282, PostErrorCode.ContentTooLong)]

    public async Task CreatePostAsync_VariousLengths_ShouldReturnExpectedError(int length, PostErrorCode expectedError)
    {
        using var transaction = db.Database.BeginTransaction();

        var user = new User
        {
            Name = "テストユーザー",
            Email = "test@example.com",
            PassWordHash = "dummyhash"
        };

        db.Users.Add(user);
        db.SaveChanges();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);
        var createPostService = new CreatePostService(db, postErrorService);

        var content = new string('あ', length);
        var result = await createPostService.CreateAsync(user.Id, content, null);

        Assert.Equal(expectedError, result.ErrorCode);

        if (expectedError == PostErrorCode.None)
        {
            Assert.NotNull(result.Post);
            Assert.Equal(content, result.Post!.Content);
        }
        else
        {
            Assert.Null(result.Post);
        }

        transaction.Rollback();
    }

}
