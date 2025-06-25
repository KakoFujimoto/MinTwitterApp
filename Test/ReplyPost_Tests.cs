using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.Tests.Common;

namespace MinTwitterApp.Tests;

public class ReplyPost_Tests : IDisposable
{
    private readonly ApplicationDbContext db;
    private readonly DateTimeAccessorForUnitTest dateTimeAccessorForUnitTest;

    public ReplyPost_Tests()
    {
        db = TestDbHelper.CreateDbContext();
        dateTimeAccessorForUnitTest = new DateTimeAccessorForUnitTest();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task ReplyPost_ShouldCreateReplyToPost()
    {
        using var transaction = db.Database.BeginTransaction();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);
        var createPostService = new CreatePostService(db, postErrorService, dateTimeAccessorForUnitTest);

        var user = User.Create(dateTimeAccessorForUnitTest, "投稿者", "reply@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var (originalError, originalPostDto) = await createPostService.CreateAsync(user.Id, "元の投稿", null);
        Assert.Equal(PostErrorCode.None, originalError);
        Assert.NotNull(originalPostDto);

        var replyService = new ReplyPostService(db, postErrorService, dateTimeAccessorForUnitTest);
        var (replyError, replyPostDto) = await replyService.CreateRepyAsync(user.Id, originalPostDto!.Id, "返信内容");

        Assert.Equal(PostErrorCode.None, replyError);
        Assert.NotNull(replyPostDto);
        Assert.Equal("返信内容", replyPostDto.Content);
        Assert.Equal(originalPostDto.Id, replyPostDto.ReplyToPostId);

        transaction.Rollback();
    }

}