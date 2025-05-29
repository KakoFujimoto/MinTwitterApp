using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;

namespace MinTwitterApp.Tests;

public static class TestDbHelper
{
    private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=MinTwitterAppDb-Test;Trusted_Connection=True;";

    public static ApplicationDbContext CreateDbContext()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlServer(ConnectionString);
        return new ApplicationDbContext(builder.Options);
    }

    static TestDbHelper()
    {
        using var db = CreateDbContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
}