using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;
using MinTwitterApp.Filters;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;

[Authorize]
[UnauthorizedExceptionFilter]
public class CreatePostController : Controller
{
    private readonly CreatePostService _createPostService;

    private readonly ViewPostService _viewPostService;

    private readonly IPostErrorMessages _postErrorMessages;

    private readonly LoginUser _loginUser;

    public CreatePostController(
        CreatePostService createPostService,
        ViewPostService viewPostService,
        IPostErrorMessages postErrorMessages,
        LoginUser loginUser)
    {
        _createPostService = createPostService;
        _viewPostService = viewPostService;
        _postErrorMessages = postErrorMessages;
        _loginUser = loginUser;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {

        int currentUserId = _loginUser.GetUserId();

        var posts = await _viewPostService.GetAllPostsAsync(currentUserId);

        var dto = new CreatePostDTO
        {
            Posts = posts,
            CurrentUserId = currentUserId
        };

        return View("Create", dto);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CreatePostDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var currentUserId = _loginUser.GetUserId();
            dto.Posts = await _viewPostService.GetAllPostsAsync(currentUserId);
            return View("Create", dto);
        }

        var inputUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(inputUserId, out int userId))
        {
            return Unauthorized();
        }

        var (errorCode, post) = await _createPostService.CreateAsync(userId, dto.Content, dto.ImageFile);

        if (errorCode != Enums.PostErrorCode.None)
        {
            var errorMessage = _postErrorMessages.GetErrorMessage(errorCode);

            if (errorCode == Enums.PostErrorCode.ContentEmpty)
            {
                ModelState.AddModelError(nameof(dto.Content), errorMessage);
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
        }

        if (!ModelState.IsValid)
        {
            int? currentUserId = _loginUser.GetUserId();
            dto.Posts = await _viewPostService.GetAllPostsAsync(currentUserId.Value);
            return View("Create", dto);
        }

        return RedirectToAction("Index", "CreatePost");
    }
}
