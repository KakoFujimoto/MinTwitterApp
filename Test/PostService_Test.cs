using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;

namespace MinTwitterApp.Tests;

public class PostService_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    public PostService_Tests()
    {
        db = TestDbHelper.CreateDbContext();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public void CreatePost_Empty_ShoudReturnError()
    {
        var postService = new PostService(db);

        var result = postService.CreatePost(1, "", null);

        Assert.Equal(PostErrorCode.ContentEmpty, result.ErrorCode);
        Assert.Null(result.Post);
    }

    [Fact]
    public void CreatePost_Ok_Test()
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

        var postService = new PostService(db);

        var result = postService.CreatePost(1, "テスト投稿", null);

        Assert.Equal(PostErrorCode.None, result.ErrorCode);
        Assert.NotNull(result.Post);
        Assert.Equal(1, result.Post!.UserId);
        Assert.Equal("テスト投稿", result.Post.Content);

        transaction.Rollback();
    }

}