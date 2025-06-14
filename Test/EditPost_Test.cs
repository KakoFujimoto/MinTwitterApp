using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;

namespace MinTwitterApp.Tests;

public class EditPost_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    public EditPost_Tests()
    {
        db = TestDbHelper.CreateDbContext();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task EditPost_Success_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var user = User.Create("編集ユーザー", "edit@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, new PostErrorService());
        (PostErrorCode errorCode, PostPageDTO? postDto) = await createPostService.CreateAsync(user.Id, "編集前の内容", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var editPostService = new EditPostService(db);

        var result = await editPostService.EditAsync(postDto!.Id, "編集後の内容");

        Assert.Equal(PostErrorCode.None, result);

        var editedPost = db.Posts.First(p => p.Id == postDto.Id);
        Assert.Equal("編集後の内容", editedPost.Content);

        transaction.Rollback();
    }

    [Fact]
    public async Task EditPost_NotFound_Test()
    {
        var editPostService = new EditPostService(db);

        var result = await editPostService.EditAsync(-999, "存在しない投稿の編集");

        Assert.Equal(PostErrorCode.NotFound, result);
    }

    [Fact]
    public async Task EditPost_DeletedPost_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var user = User.Create("削除済ユーザー", "deleted@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, new PostErrorService());
        (PostErrorCode errorCode, PostPageDTO? postDto) = await createPostService.CreateAsync(user.Id, "削除される投稿", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var deleteService = new DeletePostService(db);
        await deleteService.DeleteAsync(postDto!.Id);

        var editPostService = new EditPostService(db);
        var result = await editPostService.EditAsync(postDto.Id, "編集しようとした内容");

        Assert.Equal(PostErrorCode.AlreadyDeleted, result);

        transaction.Rollback();
    }
}
