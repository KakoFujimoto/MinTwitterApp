using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;

namespace MinTwitterApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditPostController : ControllerBase
{
    private readonly EditPostService _editPostService;
    private readonly IPostErrorMessages _errorMessages;

    public EditPostController(EditPostService editPostService, IPostErrorMessages errorMessages)
    {
        _editPostService = editPostService;
        _errorMessages = errorMessages;
    }

    public class EditPostRequest
    {
        public string Content { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public bool DeleteImage { get; set; }
    }

    [HttpPut("{postId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Edit(int postId, [FromForm] EditPostRequest request)
    {
        if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "入力内容に誤りがあります。" });
            }

            var result = await _editPostService.EditAsync(
                postId,
                request.Content,
                request.ImageFile,
                request.DeleteImage
            );

            if (result == PostErrorCode.None)
            {
                return Ok(new { message = "投稿を編集しました" });
            }

            var errorMessage = _errorMessages.GetErrorMessage(result);
            return BadRequest(new { error = errorMessage });
        }
}
