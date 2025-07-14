using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;
using MinTwitterApp.Filters;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;

[Authorize]
[UnauthorizedExceptionFilter]
[Route("User")]
public class UserProfileController : Controller
{
    private readonly ViewPostService _viewPostService;

    private readonly UserProfileService _userProfileService;

    private readonly LoginUser _loginuser;
    public UserProfileController(ViewPostService viewPostService, UserProfileService userProfileService, LoginUser loginUser)
    {
        _viewPostService = viewPostService;
        _userProfileService = userProfileService;
        _loginuser = loginUser;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Index(int id)
    {
        var currentUserId = _loginuser.GetUserId();

        var profile = await _userProfileService.GetUserProfileAsync(id);
        if (profile == null)
        {
            return NotFound();
        }

        var posts = await _viewPostService.GetPostsAsync(currentUserId, filterUserId: id);

        var model = new UserProfilePageDTO
        {
            Profile = profile,
            Posts = posts,
            IsCurrentUser = currentUserId == profile.UserId,
            CurrentUserId = currentUserId
        };

        return View("Profile", model);
    }
}