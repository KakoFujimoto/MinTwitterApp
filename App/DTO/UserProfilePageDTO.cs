using MinTwitterApp.Models;

namespace MinTwitterApp.DTO;

public class UserProfilePageDTO
{
    public UserProfileDTO Profile { get; set; } = null!;

    public List<PostPageDTO> Posts { get; set; } = new();

    public bool IsCurrentUser { get; set; }
    public int CurrentUserId { get; set; }
}