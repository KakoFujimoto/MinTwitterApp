using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Tests;

public class DeletePost_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    private readonly DateTimeAccessorForUnitTest dateTimeAccessorForUnitTest;

    public DeletePost_Tests()
    {
        db = TestDbHelper.CreateDbContext();
        dateTimeAccessorForUnitTest = new DateTimeAccessorForUnitTest();
    }

    public void Dispose()
    {
        db.Dispose();
    }


    [Fact]
    public async Task DeletePost_Success_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);

        var user = User.Create(dateTimeAccessorForUnitTest, "削除ユーザー", "delete@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, postErrorService);
        var (errorCode, postDto) = await createPostService.CreateAsync(user.Id, "削除対象", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var deleteService = new DeletePostService(db);
        var result = await deleteService.DeleteAsync(postDto!.Id);

        Assert.Equal(PostErrorCode.None, result);

        var deletePost = db.Posts.IgnoreQueryFilters().First(p => p.Id == postDto.Id);
        Assert.True(deletePost.IsDeleted);

        transaction.Rollback();
    }

    [Fact]
    public async Task DeletePost_NotFound_Test()
    {
        var deleteService = new DeletePostService(db);
        var result = await deleteService.DeleteAsync(-999);
        Assert.Equal(PostErrorCode.NotFound, result);
    }

    [Fact]
    public async Task DeletePost_AlreadyDeleted_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);

        var user = User.Create(dateTimeAccessorForUnitTest, "既に削除済", "delete@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, postErrorService);
        var (errorCode, postDto) = await createPostService.CreateAsync(user.Id, "既に削除された投稿", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var deleteService = new DeletePostService(db);

        var firstResult = await deleteService.DeleteAsync(postDto!.Id);
        Assert.Equal(PostErrorCode.None, firstResult);

        var secondResult = await deleteService.DeleteAsync(postDto.Id);
        Assert.Equal(PostErrorCode.NotFound, secondResult);

        transaction.Rollback();
    }
}
