using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using MinTwitterApp.Enums;

namespace MinTwitterApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReplyPostController : ControllerBase
{
    private readonly ReplyPostService _replyPostService;

    private readonly IPostErrorMessages _postErrorMessages;

    public ReplyPostController(ReplyPostService replyPostService, IPostErrorMessages postErrorMessages)
    {
        _replyPostService = replyPostService;
        _postErrorMessages = postErrorMessages;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReply([FromBody] CreateReplyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (errorCode, replyPost) = await _replyPostService.ReplyPostAsync(
            request.UserId,
            request.OriginalPostId,
            request.Content
        );

        if (errorCode == PostErrorCode.None)
        {
            return Ok(replyPost);
        }

        var message = _postErrorMessages.GetErrorMessage(errorCode);

        return errorCode switch
        {
            PostErrorCode.NotFound => NotFound(message),
            PostErrorCode.ContentEmpty or PostErrorCode.ContentTooLong or
            PostErrorCode.InvalidImageExtension or PostErrorCode.InvalidImageFormat or
            PostErrorCode.ImageReadError => BadRequest(message),
            _ => StatusCode(500, message)
        };
    }
}