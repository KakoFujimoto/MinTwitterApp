using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.Tests.Common;

namespace MinTwitterApp.Tests;

public class User_Tests : IDisposable
{
    private readonly ApplicationDbContext db;

    public User_Tests()
    {
        db = TestDbHelper.CreateDbContext();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public void CreateUser_Ok_Test()
    {
        var name = "testuser";
        var email = "test@example.com";
        var passwordHash = "hashedPassword";

        // 中でDateTime.UtcNowを使わずに、IDateTimeAccessorを受け取って使うようにする
        //  - 稼働時はDateTimeAccessorを利用する
        //  - テスト中はDateTimeAccessorForUnitTestを利用する
        var dateTimeAccessor = new DateTimeAccessorForUnitTest();
        var user = User.Create(dateTimeAccessor, name, email, passwordHash);

        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PassWordHash);
        // Assert.True((DateTime.UtcNow - user.CreatedAt).TotalSeconds < 5);
        Assert.Equal(dateTimeAccessor.Now, user.CreatedAt);
    }

    // テストだけのコード
    // class DateTimeAccessorForUnitTest
    //     : IDateTimeAccessor
    // {
    //     public DateTime Now => new DateTime(2000, 2, 3, 4, 5, 6);
    // }



    [Fact]
    public void DuplicateEmail_Error_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        
        var dateTimeAccessor = new DateTimeAccessorForUnitTest();

        var user1 = User.Create(dateTimeAccessor,"User1", "test@example.com", "pw1");
        var user2 = User.Create(dateTimeAccessor,"User2", "test@example.com", "pw2");

        db.Users.Add(user1);
        db.Users.Add(user2);

        Assert.Throws<DbUpdateException>(() => db.SaveChanges());

        transaction.Rollback();
    }

    [Fact]
    public async Task Login_Successful_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userErrorService = new UserErrorService(db);
        var userService = new UserService(db, passwordService, userErrorService);
        var authService = new AuthService(db, passwordService, userService);

        var dateTimeAccessor = new DateTimeAccessorForUnitTest();
        var rawPassword = "examplepassword";
        var hashedPassword = passwordService.Hash(rawPassword);
        var user = User.Create(dateTimeAccessor,"testuser", "test@example.com", hashedPassword);

        db.Users.Add(user);
        db.SaveChanges();

        var loggedInUser = await authService.LoginAsync("test@example.com", rawPassword);

        Assert.NotNull(loggedInUser);
        Assert.Equal("testuser", loggedInUser!.Name);

        transaction.Rollback();
    }

    [Fact]
    public async Task Login_Failure_InvalidEmail_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userErrorService = new UserErrorService(db);
        var userService = new UserService(db, passwordService, userErrorService);
        var authService = new AuthService(db, passwordService, userService);

        var dateTimeAccessor = new DateTimeAccessorForUnitTest();
        var rawPassword = "examplepassword";
        var hashedPassword = passwordService.Hash(rawPassword);
        var user = User.Create(dateTimeAccessor,"testuser", "test@example.com", hashedPassword);

        db.Users.Add(user);
        db.SaveChanges();

        var result = await authService.LoginAsync("wrong@example.com", rawPassword);

        Assert.Null(result);

        transaction.Rollback();
    }

    [Fact]
    public async Task Login_Failure_InValidPassword_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userErrorService = new UserErrorService(db);
        var userService = new UserService(db, passwordService, userErrorService);
        var authService = new AuthService(db, passwordService, userService);

        var dateTimeAccessor = new DateTimeAccessorForUnitTest();
        var rawPassword = "examplepassword";
        var wrongPassword = "wrongpassword";
        var hashedPassword = passwordService.Hash(rawPassword);
        var user = User.Create(dateTimeAccessor,"testuser", "test@example.com", hashedPassword);

        db.Users.Add(user);
        db.SaveChanges();

        var result = await authService.LoginAsync("test@example.com", wrongPassword);

        Assert.Null(result);

        transaction.Rollback();
    }

    [Fact]
    public async Task Register_NameEmpty_ReturnsNameEmptyError()
    {
        var passwordService = new PasswordService();
        var userErrorService = new UserErrorService(db);
        var userService = new UserService(db, passwordService, userErrorService);

        var result = await userService.RegisterAsync("", "test@example.com", "validPassword");

        Assert.Equal(UserRegisterErrorCode.NameEmpty, result);
    }

    [Fact]
    public async Task Register_EmailEmpty_ReturnsEmailEmptyError()
    {
        var passwordService = new PasswordService();
        var userErrorService = new UserErrorService(db);
        var userService = new UserService(db, passwordService, userErrorService);

        var result = await userService.RegisterAsync("testuser", " ", "validPassword");

        Assert.Equal(UserRegisterErrorCode.EmailEmpty, result);
    }

}
