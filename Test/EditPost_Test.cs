using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;
using MinTwitterApp.Tests.Common;

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

    class DateTimeAccessorForUnitTest : IDateTimeAccessor
    {
        public DateTime Now => new DateTime(2000, 2, 3, 4, 5, 6);
    }

    [Fact]
    public async Task EditPost_Success_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var dateTimeAccessor = new DateTimeAccessorForUnitTest();
        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);

        var user = User.Create(dateTimeAccessor, "編集ユーザー", "edit@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, postErrorService);
        (PostErrorCode errorCode, PostPageDTO? postDto) = await createPostService.CreateAsync(user.Id, "編集前の内容", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var editPostService = new EditPostService(db, postErrorService);
        var result = await editPostService.EditAsync(postDto!.Id, "編集後の内容", null, false);

        Assert.Equal(PostErrorCode.None, result);

        var editedPost = db.Posts.First(p => p.Id == postDto.Id);
        Assert.Equal("編集後の内容", editedPost.Content);

        transaction.Rollback();
    }

    [Fact]
    public async Task EditPost_NotFound_Test()
    {
        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);
        var editPostService = new EditPostService(db, postErrorService);

        var result = await editPostService.EditAsync(-999, "存在しない投稿の編集", null, false);

        Assert.Equal(PostErrorCode.NotFound, result);
    }

    [Fact]
    public async Task EditPost_DeletedPost_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var dateTimeAccessor = new DateTimeAccessorForUnitTest();
        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);

        var user = User.Create(dateTimeAccessor, "ユーザー", "deleted@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, postErrorService);
        (PostErrorCode errorCode, PostPageDTO? postDto) = await createPostService.CreateAsync(user.Id, "削除される投稿", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var deleteService = new DeletePostService(db);
        await deleteService.DeleteAsync(postDto!.Id);

        var editPostService = new EditPostService(db, postErrorService);
        var result = await editPostService.EditAsync(postDto.Id, "編集しようとした内容", null, false);

        Assert.Equal(PostErrorCode.NotFound, result);

        transaction.Rollback();
    }
}
