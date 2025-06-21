using System.ComponentModel.DataAnnotations;

namespace MinTwitterApp.DTO;

public class CreatePostDTO
{
    [Required(ErrorMessage = "投稿内容は必須です。")]
    // [StringLength(280, ErrorMessage = "内容は280文字以内で入力してください。")]
    public string Content { get; set; } = string.Empty;

    public string? ImagePath { get; set; }

    public IFormFile? ImageFile { get; set; }

    public int CurrentUserId { get; set; }

    public List<PostPageDTO> Posts { get; set; } = new();
}