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

    [Fact]
    public async Task CreatePostAsync_WithTooManyStrings_ShouldReturnError()
    {
        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);
        var createPostService = new CreatePostService(db, postErrorService);

        var longContent = new String('あ', 281);
        var result = await createPostService.CreateAsync(1, longContent, null);

        Assert.Equal(PostErrorCode.ContentTooLong, result.ErrorCode);
        Assert.Null(result.Post);
    }

}
