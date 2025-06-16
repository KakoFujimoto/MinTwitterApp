using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Tests.Common;

namespace MinTwitterApp.Tests;

public class UserService_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    public UserService_Tests()
    {
        db = TestDbHelper.CreateDbContext();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Register_Succeeds()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userErrorService = new UserErrorService(db);
        var userService = new UserService(db, passwordService, userErrorService);

        var errorCode = await userService.RegisterAsync("TestUser", "test@example.com", "password");

        Assert.Equal(UserRegisterErrorCode.None, errorCode);

        var registeredUser = db.Users.FirstOrDefault(u => u.Email == "test@example.com");
        Assert.NotNull(registeredUser);
        Assert.Equal("TestUser", registeredUser!.Name);
        Assert.NotEqual("password", registeredUser.PassWordHash);

    }

    [Fact]
    public async Task Register_Fails_WhenEmailAlreadyExists()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userErrorService = new UserErrorService(db);
        var userService = new UserService(db, passwordService, userErrorService);

        var dateTimeAccessor = new DateTimeAccessorForUnitTest();
        var existingUser = User.Create(dateTimeAccessor, "A", "a@example.com", passwordService.Hash("pass123"));
        db.Users.Add(existingUser);
        db.SaveChanges();

        var errorCode = await userService.RegisterAsync("A2", "a@example.com", "pass456");

        Assert.Equal(UserRegisterErrorCode.EmailAlreadyExists, errorCode);

        transaction.Rollback();
    }

        // XXX: インターフェースを使うとテスト用のダミークラスを用意できる
        //      ※PostErrorMessagesクラスが未実装でもテストが書ける
        // IPostErrorMessages postErrorMessage = new PostErrorMessagesStub();

        // XXX: Moqを使った場合
        // var postErrorMessageMock = new Mock<IPostErrorMessages>();
        // postErrorMessageMock.Setup(
        //     x => x.GetErrorMessage(It<PostErrorCode>.Any())).Returns("未実装");
        // postErrorMessageMock.Setup(
        //     x => x.GetErrorMessage(PostErrorCode.AlreadyDeleted)).Returns("AlreadyDeleted");
        //     var postErrorMessage = postErrorMessageMock.Object;

        //     var controller = new Controllers.CreatePostController(
        //         new CreatePostService(db, new PostErrorService()),
        //         new ViewPostService(db),
        //         postErrorMessage);
        // }

        // XXX: インターフェースを使うとテスト用のダミークラスを用意できる
    class PostErrorMessagesStub
        : IPostErrorMessages
    {
        public string GetErrorMessage(PostErrorCode errorCode)
        {
            return "未実装";
        }
    }
}