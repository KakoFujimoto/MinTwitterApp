using MinTwitterApp.Enums;
using MinTwitterApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MinTwitterApp.Services;

public class UserErrorService
{
    private readonly ApplicationDbContext _db;

    public UserErrorService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserRegisterErrorCode> ValidateUserInputAsync(
        string name,
        string email,
        string password
    )
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
        return UserRegisterErrorCode.None;
    }

}