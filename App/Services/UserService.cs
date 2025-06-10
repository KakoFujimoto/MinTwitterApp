using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordService _passwordService;

    public UserService(ApplicationDbContext db, PasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
    }

    public async Task<UserRegisterErrorCode> RegisterAsync(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UserRegisterErrorCode.NameEmpty;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return UserRegisterErrorCode.EmailEmpty;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return UserRegisterErrorCode.PasswordEmpty;
        }

        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            return UserRegisterErrorCode.EmailAlreadyExists;
        }

        var hash = _passwordService.Hash(password);

        User user;
        try
        {
            user = User.Create(name, email, hash);
        }
        catch
        {
            return UserRegisterErrorCode.UnknownError;
        }

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return UserRegisterErrorCode.None;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

}