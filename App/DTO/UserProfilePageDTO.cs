using MinTwitterApp.Models;

namespace MinTwitterApp.DTO;

public class UserProfilePageDTO
{
    public UserProfileDTO Profile { get; set; } = null!;

    public List<Post> Posts { get; set; } = new();
}