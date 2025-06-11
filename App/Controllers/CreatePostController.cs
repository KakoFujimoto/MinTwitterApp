using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;

[Authorize]
public class CreatePostController : Controller
{
    private readonly CreatePostService _createPostService;

    public CreatePostController(CreatePostService createPostService)
    {
        _createPostService = createPostService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("Create", new CreatePostDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CreatePostDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", dto);
        }

        var inputUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(inputUserId, out int userId))
        {
            return Unauthorized();
        }

        var (errorCode, post) = await _createPostService.CreateAsync(userId, dto.Content, dto.ImageFile);

        if (errorCode == Enums.PostErrorCode.ContentEmpty)
        {
            ModelState.AddModelError(nameof(dto.Content), "内容を入力してください。");
        }
        else if (errorCode != Enums.PostErrorCode.None)
        {
            ModelState.AddModelError("", "投稿に失敗しました。");
        }

        if (!ModelState.IsValid)
        {
            return View("Create", dto);
        }

        return RedirectToAction("Index", "CreatePost");
    }
}
