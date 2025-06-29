using MinTwitterApp.Enums;

namespace MinTwitterApp.DTO;

public class FollowResultDTO
{
    public bool IsFollowing { get; set; }

    public PostErrorCode ErrorCode { get; set; } = PostErrorCode.None;
}
