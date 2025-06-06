using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;

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
    public void Register_Succeeds()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var userService = new UserService(db, passwordService);

        var errorCode = userService.Register("TestUser", "test@example.com", "password");

        Assert.Equal(RegisterErrorCode.None, errorCode);

        var registeredUser = db.Users.FirstOrDefault(u => u.Email == "test@example.com");
        Assert.NotNull(registeredUser);
        Assert.Equal("TestUser", registeredUser!.Name);
        Assert.NotEqual("password", registeredUser.PassWordHash);

    }

    [Fact]
    public void Register_Fails_WhenEmailAlreadyExists()
    {
        using var transaction = db.Database.BeginTransaction();

        var passwordService = new PasswordService();
        var UserService = new UserService(db, passwordService);

        var existingUser = User.Create("A", "a@example.com", passwordService.Hash("pass123"));
        db.Users.Add(existingUser);
        db.SaveChanges();

        var errorCode = UserService.Register("A2", "a@example.com", "pass456");

        Assert.Equal(RegisterErrorCode.EmailAlreadyExists, errorCode);

        transaction.Rollback();
    }
}