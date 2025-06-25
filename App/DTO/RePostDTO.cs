namespace MinTwitterApp.DTO;

public class RePostDTO
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public string? ImagePath { get; set; }

    public int UserId { get; set; }

    public string? UserName { get; set; }

    public int RePostSourceId { get; set; }

    public string? SourceUserName { get; set; }

    public string? SourceContent { get; set; }

    public DateTime CreatedAt { get; set; }
}