namespace MinTwitterApp.DTO;

public class UserProfileDTO
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
