using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;

namespace MinTwitterApp.Controllers;

public class FollowRelationshipController : Controller
{
    private readonly FollowUserService _followUserService;

    public FollowRelationshipController(FollowUserService followUserService)
    {
        _followUserService = followUserService;
    }
    public async Task<IActionResult> Following(int id)
    {
        var followingUsers = await _followUserService.GetFollowingUserAsync(id);
        return View(followingUsers);
    }

    public async Task<IActionResult> Followers(int id)
    {
        var followerUsers = await _followUserService.GetFollowerUserAsync(id);
        return View(followerUsers);
    }
}