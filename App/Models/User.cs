using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Common;

namespace MinTwitterApp.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public required string PassWordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();

    public ICollection<Follow> Following { get; set; } = new List<Follow>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();

    public static User Create(IDateTimeAccessor dateTimeAccessor, string name, string email, string passwordHash)
    {
        return new User
        {
            Name = name,
            Email = email,
            PassWordHash = passwordHash,
            CreatedAt = dateTimeAccessor.Now
        };
    }
}