using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Services;

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
    // EFのテストになってしまっているので不要かも
    public void CreateUser_Ok_Test()
    {
        // using var transaction = db.Database.BeginTransaction();

        var user = User.Create("testuser", "test@example.com", "hashedpassword");
        // db.Users.Add(user);
        // db.SaveChanges();

        // var savedUser = db.Users.First();
        Assert.Equal("testuser", savedUser.Name);
        Assert.Equal("test@example.com", savedUser.Email);

        // transaction.Rollback();
    }

    // EFのテストになってしまっているので不要かも
    [Fact]
    public void DuplicateEmail_Error_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var user1 = User.Create("User1", "test@example.com", "pw1");
        var user2 = User.Create("User2", "test@example.com", "pw2");

        db.Users.Add(user1);
        db.Users.Add(user2);

        Assert.Throws<DbUpdateException>(() => db.SaveChanges());

        transaction.Rollback();

    }

    [Fact]
    public void Login_Successful_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userService = new UserService(db, passwordService);
        var authService = new AuthService(db, passwordService, userService);

        var rawPassword = "examplepassword";
        var hashedPassword = passwordService.Hash(rawPassword);
        var user = User.Create("testuser", "test@example.com", hashedPassword);

        db.Users.Add(user);
        db.SaveChanges();

        var loggedInUser = authService.Login("test@example.com", rawPassword);

        Assert.NotNull(loggedInUser);
        Assert.Equal("testuser", loggedInUser!.Name);

        transaction.Rollback();
    }

    [Fact]
    public void Login_Failure_InvalidEmail_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userService = new UserService(db, passwordService);
        var authService = new AuthService(db, passwordService, userService);

        var rawPassWord = "examplepassword";
        var hashedPassword = passwordService.Hash(rawPassWord);
        var user = User.Create("testuser", "test@example.com", hashedPassword);

        db.Users.Add(user);
        db.SaveChanges();

        var result = authService.Login("wrong@example.com", rawPassWord);

        Assert.Null(result);

        transaction.Rollback();

    }

    [Fact]
    public void Login_Failure_InValidPassword_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userService = new UserService(db, passwordService);
        var authService = new AuthService(db, passwordService, userService);

        var rawPassWord = "examplepassword";
        var wrongPassword = "wrongpassword";
        var hashedPassword = passwordService.Hash(rawPassWord);
        var user = User.Create("testuser", "test@example.com", hashedPassword);

        db.Users.Add(user);
        db.SaveChanges();

        var result = authService.Login("test@example.com", wrongPassword);

        Assert.Null(result);

    }

}