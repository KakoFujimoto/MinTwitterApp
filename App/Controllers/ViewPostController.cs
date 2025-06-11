using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;

[Authorize]
public class ViewPostController : Controller
{
    private readonly ViewPostService _viewPostService;

    public ViewPostController(ViewPostService viewPostService)
    {
        _viewPostService = viewPostService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var posts = await _viewPostService.GetAllPostsAsync();
        return View("Index", posts); // Views/ViewPost/Index.cshtml を想定
    }

    [HttpGet("ViewPost/User/{userId}")]
    public async Task<IActionResult> ByUser(int userId)
    {
        var posts = await _viewPostService.GetPostsByUserIdAsync(userId);
        return View("UserPosts", posts); // Views/ViewPost/UserPosts.cshtml を想定
    }
}
