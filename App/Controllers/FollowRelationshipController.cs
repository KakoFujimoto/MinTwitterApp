using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Common;
using MinTwitterApp.DTO;
using MinTwitterApp.Services;

namespace MinTwitterApp.Controllers;

public class FollowRelationshipController : Controller
{
    private readonly FollowUserService _followUserService;
    private readonly LoginUser _loginUser;

    public FollowRelationshipController(FollowUserService followUserService, LoginUser loginUser)
    {
        _followUserService = followUserService;
        _loginUser = loginUser;
    }
    public async Task<IActionResult> Following(int id)
    {
        var followingUsers = await _followUserService.GetFollowingUserAsync(id);

        var model = new FollowingsPageDTO
        {
            Following = followingUsers
        };

        return View(model);
    }


    public async Task<IActionResult> Followers(int id)
    {
        var followerUsers = await _followUserService.GetFollowerUserAsync(id);
        var CurrentUserId = _loginUser.GetUserId();

        var model = new FollowersPageDTO
        {
            CurrentUserId = CurrentUserId,
            Followers = followerUsers
        };

        return View(model);
    }
}