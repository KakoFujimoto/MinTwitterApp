namespace MinTwitterApp.Models;

public class Follow
{
    // フォローする人
    public int FollowerId { get; set; }
    public User? Follower { get; set; }

    // フォローされる人
    public int FolloweeId { get; set; }
    public User? Followee { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}