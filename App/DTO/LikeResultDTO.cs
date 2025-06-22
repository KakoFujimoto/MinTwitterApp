using MinTwitterApp.Enums;

namespace MinTwitterApp.DTO;

public class LikeResultDTO
{
    public bool IsLiked { get; set; }

    public int LikeCount { get; set; }
    public PostErrorCode ErrorCode { get; set; } = PostErrorCode.None;
}
