namespace MinTwitterApp.DTO;

public class PostPageDTO
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;

    public string? ImagePath { get; set; }


    public int UserId { get; set; }

    public string? UserName { get; set; }

    public DateTime CreatedAt { get; set; }

    public int LikeCount { get; set; }

    public bool IsLiked { get; set; }

    public int? RepostSourceId { get; set; }

    public string? SourceUserName { get; set; }

    public string? SourceContent { get; set; }

    public string? SourceImagePath { get; set; }

}