using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using Microsoft.AspNetCore.Authorization;
using MinTwitterApp.Common;
using MinTwitterApp.Filters;

namespace MinTwitterApp.Controllers;

[Authorize]
[UnauthorizedExceptionFilter]
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
        var currentUserId = _loginuser.GetUserId();

        var posts = await _viewPostService.GetPostsAsync(currentUserId);
        return View("Index", posts);
    }

    [HttpGet("ViewPost/User/{userId}")]
    public async Task<IActionResult> ByUser(int userId)
    {
        var posts = await _viewPostService.GetPostsByUserIdAsync(userId);
        return View("UserPosts", posts);
    }
}
