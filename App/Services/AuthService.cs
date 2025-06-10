using MinTwitterApp.Data;
using MinTwitterApp.Models;

namespace MinTwitterApp.Services;

public class AuthService
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordService _passwordService;

    private readonly UserService _userService;

    public AuthService(ApplicationDbContext db, PasswordService passwordService, UserService userService)
    {
        _db = db;
        _passwordService = passwordService;
        _userService = userService;
    }

    public async Task<User?> LoginAsync(string email, string plainPassword)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null) return null;

        return _passwordService.Verify(user.PassWordHash, plainPassword) ? user : null;
    }
}