using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;
using MinTwitterApp.Common;
using MinTwitterApp.Filters;

namespace MinTwitterApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly LikePostService likePostService;

    private readonly LoginUser loginUser;

    public LikeController(LikePostService likePostService, LoginUser loginUser)
    {
        this.likePostService = likePostService;
        this.loginUser = loginUser;
    }

    [HttpPost("toggle")]
    public async Task<ActionResult<LikeResultDTO>> ToggleLike([FromBody] ToggleLikeRequest request)
    {
        var userId = loginUser.GetUserId();

        var result = await likePostService.ToggleLikeAsync(userId, request.PostId);
        return Ok(result);
    }

    [HttpGet("count/{postId}")]
    public ActionResult<int> GetLikeCount(int postId)
    {
        var count = likePostService.GetLikeCount(postId);
        return Ok(count);
    }
}
