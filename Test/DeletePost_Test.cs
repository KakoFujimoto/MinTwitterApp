using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;

namespace MinTwitterApp.Tests;

public class DeletePost_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    public DeletePost_Tests()
    {
        db = TestDbHelper.CreateDbContext();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task DeletePost_Success_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var user = User.Create("削除ユーザー", "delete@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, new PostErrorService());
        (PostErrorCode errorCode, PostPageDTO? postDto) = await createPostService.CreateAsync(user.Id, "削除対象", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var deleteService = new DeletePostService(db);

        var result = await deleteService.DeleteAsync(postDto!.Id);

        Assert.Equal(PostErrorCode.None, result);

        var deletePost = db.Posts.First(p => p.Id == postDto.Id);
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

        var user = User.Create("既に削除済", "delete@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, new PostErrorService());
        (PostErrorCode errorCode, PostPageDTO? postDto) = await createPostService.CreateAsync(
            user.Id, "既に削除された投稿", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var deleteService = new DeletePostService(db);

        var firstResult = await deleteService.DeleteAsync(postDto!.Id);
        Assert.Equal(PostErrorCode.None, firstResult);

        var secondResult = await deleteService.DeleteAsync(postDto.Id);
        Assert.Equal(PostErrorCode.AlreadyDeleted, secondResult);

        transaction.Rollback();
    }
}
