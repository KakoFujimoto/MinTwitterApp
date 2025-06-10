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
    public async Task<IActionResult> Create()
    {
        var dto = new PostCreateDTO
        {
            Posts = await _postService.GetAllPostsAsync()
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            dto.Posts = await _postService.GetAllPostsAsync();
            return View(dto);
        }

        var inputUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int result;
        bool success;

        success = int.TryParse(inputUserId,out result);

        if (!success)
        {
            return Unauthorized();
        }

        var (errorCode, post) = await _postService.CreatePostAsync(result, dto.Content, dto.ImageFile);

        if (errorCode == Enums.PostErrorCode.ContentEmpty)
        {
            ModelState.AddModelError(nameof(dto.Content), "内容を入力してください。");
            dto.Posts = await _postService.GetAllPostsAsync();
            return View(dto);
        }
        else if (errorCode != Enums.PostErrorCode.None)
        {
            ModelState.AddModelError("", "投稿に失敗しました。");
            dto.Posts = await _postService.GetAllPostsAsync();
            return View(dto);
        }
        return RedirectToAction("Create", "Post");
    }
}