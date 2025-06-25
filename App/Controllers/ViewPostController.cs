using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using Microsoft.AspNetCore.Authorization;
using MinTwitterApp.Common;

namespace MinTwitterApp.Controllers;

[Authorize]
public class ViewPostController : Controller
{
    private readonly ViewPostService _viewPostService;
    private readonly LoginUser _loginuser;

    public ViewPostController(ViewPostService viewPostService, LoginUser loginUser)
    {
        _viewPostService = viewPostService;
        _loginuser = loginUser;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!int.TryParse(_loginuser.GetUserId(), out var currentUserId))
        {
            return BadRequest("ユーザーIDが無効です。");
        }
        var posts = await _viewPostService.GetAllPostsAsync(currentUserId);
        return View("Index", posts);
    }

    [HttpGet("ViewPost/User/{userId}")]
    public async Task<IActionResult> ByUser(int userId)
    {
        var posts = await _viewPostService.GetPostsByUserIdAsync(userId);
        return View("UserPosts", posts); // Views/ViewPost/UserPosts.cshtml を想定
    }
}
