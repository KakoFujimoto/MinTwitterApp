using System.ComponentModel.DataAnnotations;

namespace MinTwitterApp.DTO;

public class LoginPageDTO
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}