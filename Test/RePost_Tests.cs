using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Tests.Common;

namespace MinTwitterApp.Tests;

public class RePost_Tests : IDisposable
{
    private readonly ApplicationDbContext db;
    private readonly DateTimeAccessorForUnitTest dateTimeAccessorForUnitTest;

    public RePost_Tests()
    {
        db = TestDbHelper.CreateDbContext();
        dateTimeAccessorForUnitTest = new DateTimeAccessorForUnitTest();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task RePost_Create_Ok()
    {
        using var transaction = db.Database.BeginTransaction();

        var imageDetector = new FakeImageFormatDetector();
        var postErrorService = new PostErrorService(imageDetector);

        var user = User.Create(dateTimeAccessorForUnitTest, "リポストユーザー", "repost@test.com", "hashedPassword");
        db.Users.Add(user);
        db.SaveChanges();


        var createPostService = new CreatePostService(db, postErrorService);
        (PostErrorCode originalErrorCode, PostPageDTO? originalPostDto) = await
        createPostService.CreateAsync(user.Id, "元の投稿", null);

        Assert.Equal(PostErrorCode.None, originalErrorCode);
        Assert.NotNull(originalPostDto);

        var repostService = new RePostService(db, dateTimeAccessorForUnitTest);
        (PostErrorCode rePostErrorCode, RePostDTO? rePostDto) = await
        repostService.RePostAsync(user.Id, originalPostDto!.Id);

        Assert.Equal(PostErrorCode.None, rePostErrorCode);
        Assert.NotNull(rePostDto);

        Assert.Equal(originalPostDto.Content, rePostDto.Content);
        Assert.NotEqual(originalPostDto.Id, rePostDto.Id);
        Assert.Equal(originalPostDto.Id, rePostDto.RePostSourceId);

        transaction.Rollback();
    }
}