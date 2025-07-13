namespace MinTwitterApp.DTO;

public class FollowingsPageDTO
{
    public int CurrentUserId { get; set; }

    public List<FollowRelationshipDTO> Following { get; set; } = new();
}