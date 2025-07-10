using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;
using MinTwitterApp.Common;
using MinTwitterApp.Filters;

namespace MinTwitterApp.Controllers;

[ApiController]
[Authorize]
[UnauthorizedExceptionFilter]
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

        int userId = _loginuser.GetUserId();

        var result = await _followUserService.ToggleFollowAsync(userId, request.TargetUserId);
        return Ok(result);
    }

    [HttpGet("IsFollowing/{TargetUserId}")]
    public async Task<ActionResult<bool>> IsFollowing(int targetUserId)
    {

        int? userId = _loginuser.GetUserId();

        var IsFollowing = await _followUserService.IsFollowingAsync(userId.Value, targetUserId);
        return Ok(IsFollowing);
    }

    [HttpPost("follow-back")]
    public async Task<ActionResult<FollowResultDTO>> FollowBack([FromBody] FollowBackRequest request)
    {

        int userId = _loginuser.GetUserId();

        var result = await _followUserService.FollowBackAsync(userId, request.TargetUserId);
        return Ok(result);
    }
}