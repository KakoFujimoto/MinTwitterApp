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
    }

    [HttpPut("{postId}")]
    public async Task<IActionResult> Edit(int postId, [FromBody] EditPostRequest request)
    {
        try
        {
            var result = await _editPostService.EditAsync(postId, request.Content);

            if (result == PostErrorCode.None)
            {
                return Ok(new { message = "投稿を編集しました" });
            }

            var errorMessage = _errorMessages.GetErrorMessage(result);
            return BadRequest(new { error = errorMessage });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

}
