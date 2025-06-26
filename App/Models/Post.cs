using System.ComponentModel.DataAnnotations;

namespace MinTwitterApp.Models;

public class Post
{
    public int Id { get; set; }

    [Required]
    [MaxLength(280)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int? RepostSourceId { get; set; }

    public int? ReplyToPostId { get; set; }

}