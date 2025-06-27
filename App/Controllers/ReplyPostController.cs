using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using MinTwitterApp.Enums;
using MinTwitterApp.Common;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReplyPostController : ControllerBase
{
    private readonly ReplyPostService _replyPostService;
    private readonly IPostErrorMessages _postErrorMessages;
    private readonly LoginUser _loginUser;

    public ReplyPostController(
        ReplyPostService replyPostService,
        IPostErrorMessages postErrorMessages,
        LoginUser loginUser)
    {
        _replyPostService = replyPostService;
        _postErrorMessages = postErrorMessages;
        _loginUser = loginUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReply([FromForm] CreateReplyRequest request)
    {
        var loginUserId = _loginUser.GetUserId();
        if (!int.TryParse(loginUserId, out int userId))
        {
            return Unauthorized("ログインユーザーが特定できません。");
        }

        var (errorCode, replyPost) = await _replyPostService.ReplyPostAsync(
            userId,
            request.OriginalPostId,
            request.Content,
            request.ImageFile
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
