using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Tests.Common;

namespace MinTwitterApp.Tests;

public class LikePost_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    public LikePost_Tests()
    {
        db = TestDbHelper.CreateDbContext();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task LikePost_ToggleLike_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var dateTimeAccessor = new DateTimeAccessorForUnitTest();

        var user = User.Create(dateTimeAccessor, "いいねするユーザー", "like@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var createPostService = new CreatePostService(db, new PostErrorService());
        (PostErrorCode errorCode, PostPageDTO? postDto) = await createPostService.CreateAsync(user.Id, "いいね対象の投稿", null);

        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var likeService = new LikePostService(db);

        // 初回
        var firstLikeResult = await likeService.ToggleLikeAsync(user.Id, postDto!.Id);
        Assert.True(firstLikeResult.IsLiked);

        // 2回目（取り消し）
        var secondLikeResult = await likeService.ToggleLikeAsync(user.Id, postDto.Id);
        Assert.False(secondLikeResult.IsLiked);

        // 3回目
        var thirdLikeResult = await likeService.ToggleLikeAsync(user.Id, postDto.Id);
        Assert.True(thirdLikeResult.IsLiked);

        // 自分の投稿にもいいねできる
        var selfLikeResult = await likeService.ToggleLikeAsync(user.Id, postDto.Id);
        Assert.False(selfLikeResult.IsLiked);

        transaction.Rollback();
    }

}