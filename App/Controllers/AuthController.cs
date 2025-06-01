using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.ViewModels;

namespace MinTwitterApp.Controllers;

public class AuthController : Controller
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
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
}