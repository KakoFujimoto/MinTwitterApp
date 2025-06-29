using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;
using MinTwitterApp.Common;

namespace MinTwitterApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FollowUserController : ControllerBase
{
    private readonly FollowUserService _followUserService;

    private readonly LoginUser _loginuser;

    public FollowUserController(FollowUserService followUserService, LoginUser loginUser)
    {
        _followUserService = followUserService;
        _loginuser = loginUser;
    }

    [HttpPost("toggle")]
    public async Task<ActionResult<FollowResultDTO>> ToggleFollow([FromBody] ToggleFollowRequest request)
    {
        var loginUserId = _loginuser.GetUserId();
        if (loginUserId == null || !int.TryParse(loginUserId, out int userId))
        {
            return Unauthorized("ログインユーザーが特定できません。");
        }
        var result = await FollowUserService.ToggleFollowAsync(userId, request.targetUserId);
        return Ok(result);
    }

    [HttpGet("IsFollowing/{TargetUserId}")]
    public async Task<ActionResult<bool>> IsFollowing(int targetUserId)
    {
        var loginUserId = _loginuser.GetUserId();
        if (loginUserId == null || int.TryParse(loginUserId, out int userId))
        {
            return Unauthorized("ログインユーザーが特定できません。");
        }

        var IsFollowing = await _followUserService.IsFollowingAsync(userId, targetUserId);
        return Ok(IsFollowing);
    }
}