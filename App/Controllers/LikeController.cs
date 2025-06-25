using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MinTwitterApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly LikePostService likePostService;

    public LikeController(LikePostService likePostService)
    {
        this.likePostService = likePostService;
    }

    [HttpPost("toggle")]
    public async Task<ActionResult<LikeResultDTO>> ToggleLike([FromBody] ToggleLikeRequest request)
    {   
        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userClaim == null || !int.TryParse(userClaim.Value, out int userId))
        {
            return Unauthorized("ログインユーザーが特定できません。");
        }
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
