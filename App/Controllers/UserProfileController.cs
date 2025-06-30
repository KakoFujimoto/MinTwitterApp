using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Models;
using MinTwitterApp.DTO;

namespace MinTwitterApp.Controllers;

[Route("User")]
public class UserProfileController : Controller
{
    private readonly UserProfileService _userProfileService;

    private readonly UserService _userService;

    public UserProfileController(UserProfileService userProfileService, UserService userService)
    {
        _userProfileService = userProfileService;
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Index(int id)
    {
        var profile = await _userProfileService.GetUserProfileAsync(id);
        if (profile == null)
        {
            return NotFound();
        }

        var posts = await _userProfileService.GetPostByUserAsync(id);

        var model = new UserProfilePageDTO
        {
            Profile = profile,
            Posts = posts
        };

        return View("Profile", model);
    }
}