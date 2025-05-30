using MinTwitterApp.Data;
using MinTwitterApp.Models;

namespace MinTwitterApp.Tests;

public class User_Test : IDisposable
{
    private readonly ApplicationDbContext db;

    public User_Test()
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
        using var transaction = db.Database.BeginTransaction();

        var user = User.Create("testuser", "test@example.com", "hashedpassword");

        db.Users.Add(user);
        db.SaveChanges();

        var savedUser = db.Users.First();
        Assert.Equal("testuser", savedUser.Name);
        Assert.Equal("test@example.com", savedUser.Email);

        transaction.Rollback();
    }

    [Fact]
    public void Login_Successful_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var password = "examplepassword";
        var user = User.Create("testuser", "test@example.com", User.HashPassword(password));

        db.Users.Add(user);
        db.SaveChanges();

        var foundUser = db.Users.FirstOrDefault(u => u.Email == "test@example.com");
        Assert.NotNull(foundUser);
        Assert.True(foundUser!.VerifyPassword(password));

        transaction.Rollback();
    }

    [Fact]
    public void Login_Failure_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var user = User.Create("testuser", "test@example.com", User.HashPassword("correctpassword"));
        db.Users.Add(user);
        db.SaveChanges();

        var foundUser = db.Users.FirstOrDefault(u => u.Email == "test@example.com");
        Assert.NotNull(foundUser);
        Assert.False(foundUser!.VefifyPassword("wrongpassword"));

        transaction.Rollback();
    }

}