using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;

namespace MinTwitterApp.Controllers;

[Route("User")]
public class UserProfileController : Controller
{
    private readonly UserProfileService _userProfileService;

    private readonly LoginUser _loginuser;
    public UserProfileController(UserProfileService userProfileService, LoginUser loginUser)
    {
        _userProfileService = userProfileService;
        _loginuser = loginUser;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Index(int id)
    {
        var loginUserId = int.TryParse(_loginuser.GetUserId(), out int userId) ? userId : -1;

        var profile = await _userProfileService.GetUserProfileAsync(id);
        if (profile == null)
        {
            return NotFound();
        }

        var posts = await _userProfileService.GetPostDtoByUserAsync(id, loginUserId);

        var model = new UserProfilePageDTO
        {
            Profile = profile,
            Posts = posts,
            IsCurrentUser = (loginUserId == profile.UserId),
            CurrentUserId = loginUserId
        };

        return View("Profile", model);
    }
}