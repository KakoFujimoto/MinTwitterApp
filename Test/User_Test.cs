using Microsoft.EntityFrameworkCore;
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

    [Fact]
    public void DuplicateEmail_Error_Test()
    {
        using var transaction = db.Database.BeginTransaction();

        var user1 = User.Create("User1", "test@example.com", "pw1");
        var user2 = User.Create("User2", "test@example.com", "pw2");

        db.Users.Add(user1);
        db.Users.Add(user2);

        Assert.Throws<DbUpdateException>(() => db.SaveChanges());

    }
}