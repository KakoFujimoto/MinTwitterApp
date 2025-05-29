using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace MinTwitterApp.Models;

public class Like
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }

    public int PostId { get; set; }

    public User User { get; set; } = null!;

    public Post Post { get; set; } = null!;
}