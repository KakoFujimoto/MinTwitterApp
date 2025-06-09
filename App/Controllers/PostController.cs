using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;

namespace MinTwitterApp.Controllers;

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
        var userId = _sessionService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            dto.Posts = _postService.GetAllPosts();
            return View(dto);
        }

        var (errorCode, post) = _postService.CreatePost(userId.Value, dto.Content, dto.ImageFile);

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