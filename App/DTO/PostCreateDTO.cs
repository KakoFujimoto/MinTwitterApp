using System.ComponentModel.DataAnnotations;

namespace MinTwitterApp.DTO;

public class PostCreateDTO
{
    [Required(ErrorMessage = "投稿内容は必須です。")]
    [StringLength(280, ErrorMessage = "内容は280文字以内で入力してください。")]
    public string Content { get; set; } = string.Empty;
}