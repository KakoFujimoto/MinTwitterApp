using MinTwitterApp.Data;
using MinTwitterApp.Models;
using MinTwitterApp.Enums;

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

    public RegisterErrorCode Register(string name, string email, string password)
    {
        if (_db.Users.Any(u => u.Email == email))
        {
            return RegisterErrorCode.EmailAlreadyExists;
        }

        var hash = _passwordService.Hash(password);
        var user = User.Create(name, email, hash);
        _db.Users.Add(user);
        _db.SaveChanges();

        return RegisterErrorCode.None;
    }

    public User? GetUserByEmail(string email)
    {
        return _db.Users.FirstOrDefault(u => u.Email == email);
    }

}