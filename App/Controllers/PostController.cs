using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly PostService _postService;
    private readonly ISessionService _sessionService;

    public PostController(PostService postService, ISessionService sessionService)
    {
        _postService = postService;
        _sessionService = sessionService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        var dto = new PostCreateDTO
        {
            Posts = _postService.GetAllPosts()
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PostCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            dto.Posts = _postService.GetAllPosts();
            return View(dto);
        }

        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var (errorCode, post) = _postService.CreatePost(userId, dto.Content, dto.ImageFile);

        if (errorCode == Enums.PostErrorCode.ContentEmpty)
        {
            ModelState.AddModelError(nameof(dto.Content), "内容を入力してください。");
            dto.Posts = _postService.GetAllPosts();
            return View(dto);
        }
        else if (errorCode != Enums.PostErrorCode.None)
        {
            ModelState.AddModelError("", "投稿に失敗しました。");
            dto.Posts = _postService.GetAllPosts();
            return View(dto);
        }
        return RedirectToAction("Create", "Post");
    }
}