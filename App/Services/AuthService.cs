using MinTwitterApp.Data;
using MinTwitterApp.Models;

namespace MinTwitterApp.Services;

public class AuthService
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordService _passwordService;

    public AuthService(ApplicationDbContext db, PasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
    }

    public User? Login(string email, string plainPassword)
    {   
        // 20行目をUserServiceに移す
        var user = _db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return null;

        return _passwordService.Verify(user.PassWordHash, plainPassword) ? user : null;
    }
}