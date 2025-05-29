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

}