using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.ViewModels;

namespace MinTwitterApp.Controllers;

public class AuthController : Controller
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public AuthController(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errorCode) = _userService.Register(model.Name, model.Email, model.Password);

        if (!success)
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

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
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

        HttpContext.Session.SetInt32("UserId", user.Id);

        return RedirectToAction("Index", "Home");

    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        TempData["Message"] = "ログアウトしました。";
        
        return RedirectToAction("Login", "Auth");
    }

}