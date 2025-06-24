using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Common;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;



[Authorize]
public class RePostController : Controller
{
    private readonly RePostService _rePostService;
    private readonly LoginUser _loginUser;

    public RePostController(RePostService rePostService, LoginUser loginUser)
    {
        _rePostService = rePostService;
        _loginUser = loginUser;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int originalPostId)
    {
        if (!int.TryParse(_loginUser.GetUserId(), out int userId))
        {
            return Unauthorized();
        }

        var (errorCode, repostDto) = await _rePostService.RePostAsync(userId, originalPostId);

        if (errorCode != Enums.PostErrorCode.None)
        {
            TempData["Message"] = "リポストに失敗しました。";
        }
        else
        {
            TempData["Message"] = "リポストしました。";
        }

        return RedirectToAction("Index", "CreatePost");
    }
}
