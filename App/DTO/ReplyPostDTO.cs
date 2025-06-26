using System.ComponentModel.DataAnnotations;

namespace MinTwitterApp.DTO;

public class ReplyPostDTO
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int userId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int ReplyToPostId { get; set; }
    public string OriginalContent { get; set; } = string.Empty;
    public string originalUserName { get; set; } = string.Empty;

}