namespace MinTwitterApp.DTO;

public class CreateReplyRequest
{
    public int UserId { get; set; }
    public int OriginalPostId { get; set; }

    public string? Content { get; set; }
}