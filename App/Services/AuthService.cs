using MinTwitterApp.Data;
using MinTwitterApp.Models;

namespace MinTwitterApp.Services;

public class AuthService
{
    private readonly ApplicationDbContext db;
    private readonly PasswordService passwordService;

    public AuthService(ApplicationDbContext db, PasswordService passwordService)
    {
        this.db = db;
        this.passwordService = passwordService;
    }

    public User? Login(string email, string plainPassword)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return null;

        return passwordService.Verify(user.PassWordHash, plainPassword) ? user : null;
    }
}