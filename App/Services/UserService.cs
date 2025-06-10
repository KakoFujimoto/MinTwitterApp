using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordService _passwordService;

    private readonly UserErrorService _userErrorService;

    public UserService(
        ApplicationDbContext db,
        PasswordService passwordService,
        UserErrorService userErrorService)
    {
        _db = db;
        _passwordService = passwordService;
        _userErrorService = userErrorService;
    }

    public async Task<UserRegisterErrorCode> RegisterAsync
        (
            string name,
            string email,
            string password
        )
    {
        var validationResult = await _userErrorService.ValidateUserInputAsync(name, email, password);

        if (validationResult != UserRegisterErrorCode.None)
        {
            return validationResult;
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