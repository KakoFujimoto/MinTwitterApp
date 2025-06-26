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
        var (replyError, replyPostDto) = await replyService.ReplyPostAsync(user.Id, originalPostDto!.Id, "返信内容");

        Assert.Equal(PostErrorCode.None, replyError);
        Assert.NotNull(replyPostDto);
        Assert.Equal("返信内容", replyPostDto.Content);
        Assert.Equal(originalPostDto.Id, replyPostDto.ReplyToPostId);

        transaction.Rollback();
    }

    [Fact]
    public async Task ReplyPost_WhenOriginalPostDeleted_ShouldReturnNotFoundError()
    {
        using var transaction = db.Database.BeginTransaction();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);
        var createPostService = new CreatePostService(db, postErrorService, dateTimeAccessorForUnitTest);
        var replyPostService = new ReplyPostService(db, postErrorService, dateTimeAccessorForUnitTest);

        var user = User.Create(dateTimeAccessorForUnitTest, "リプライユーザー", "replyuser@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();

        var (errorCode, postDto) = await createPostService.CreateAsync(user.Id, "削除された投稿", null);
        Assert.Equal(PostErrorCode.None, errorCode);
        Assert.NotNull(postDto);

        var deletedPost = db.Posts.First(p => p.Id == postDto!.Id);
        deletedPost.IsDeleted = true;
        db.SaveChanges();

        var result = await replyPostService.ReplyPostAsync(user.Id, deletedPost.Id, "リプライ本文");

        Assert.Equal(PostErrorCode.NotFound, result.errorCode);
        Assert.Null(result.Post);

        transaction.Rollback();

    }

}