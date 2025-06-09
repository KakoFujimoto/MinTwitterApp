using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;

namespace MinTwitterApp.Controllers;

public class AuthController : Controller
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    private readonly ISessionService _sessionService;

    public AuthController(UserService userService, AuthService authService, ISessionService sessionService)
    {
        _userService = userService;
        _authService = authService;
        _sessionService = sessionService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Register(RegisterPageDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var errorCode = _userService.Register(model.Name, model.Email, model.Password);

        if (errorCode != RegisterErrorCode.None)
        {
            if (errorCode == RegisterErrorCode.EmailAlreadyExists)
            {
                ModelState.AddModelError("Email", "このメールアドレスは既に登録されています。");
            }
            else
            {
                ModelState.AddModelError("", "登録に失敗しました");
            }

            return View(model);
        }

        return RedirectToAction("Login", "Auth");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(LoginPageDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _authService.Login(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError("", "メールアドレスまたはパスワードが正しくありません。");
            return View(model);
        }

        _sessionService.SetUserId(user.Id);

        return RedirectToAction("Create", "Post");

    }

    [Authorize]
    [HttpPost]
    public IActionResult Logout()
    {
        _sessionService.Clear();

        TempData["Message"] = "ログアウトしました。";

        return RedirectToAction("Login", "Auth");
    }

}