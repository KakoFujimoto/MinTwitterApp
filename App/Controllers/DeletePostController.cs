using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;

namespace MinTwitterApp.Controllers;

[ApiController]
[Route("api/[controller]")]

public class DeletePostController : ControllerBase
{
    private readonly DeletePostService _deletePostService;
    private readonly IPostErrorMessages _errorMessages;

    public DeletePostController(DeletePostService deletePostService, IPostErrorMessages errorMessages)
    {
        _deletePostService = deletePostService;
        _errorMessages = errorMessages;
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> Delete(int postId)
    {
        var result = await _deletePostService.DeleteAsync(postId);

        if (result == PostErrorCode.None)
        {
            return Ok(new { message = "投稿を削除しました" });
        }

        var errorMessage = _errorMessages.GetErrorMessage(result);
        return BadRequest(new { error = errorMessage });
    }
}