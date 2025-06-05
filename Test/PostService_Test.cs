using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;

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

    // CreateUser_Ok_Test()みたいなかんじで書くと意味がある
    // ひとまず異常系はいったん放置で正常系を先にやる
    [Fact]
    public void CreatePost_Empty_ShoudThrowException()
    {
        var postService = new PostService(db);
        Assert.Throws<ArgumentException>(() =>
        {
            postService.CreatePost(1, "");
        });
    }

    [Fact]

    public void CreatePost_Ok_Test()
    {
        var postService = new PostService(db);
        var result = postService.CreatePost(1, "テスト投稿");

        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("テスト投稿", result.Content);
    }

}