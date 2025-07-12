namespace MinTwitterApp.DTO;

public class FollowersPageDTO
{
    public int CurrentUserId { get; set; }

    public List<FollowRelationshipDTO> Followers { get; set; } = new();
}