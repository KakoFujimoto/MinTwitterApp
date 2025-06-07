using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using MinTwitterApp.Models;

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
        return View();
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
            return View(dto);

        var (errorCode, resultDto) = _postService.CreatePost(userId.Value, dto.Content);

        if (errorCode == Enums.PostErrorCode.ContentEmpty)
        {
            ModelState.AddModelError(nameof(dto.Content), "内容を入力してください。");
            return View(dto);
        }
        else if (errorCode != Enums.PostErrorCode.None)
        {
            ModelState.AddModelError("", "投稿に失敗しました。");
            return View(dto);
        }
        return RedirectToAction("Index", "Home");
    }
}