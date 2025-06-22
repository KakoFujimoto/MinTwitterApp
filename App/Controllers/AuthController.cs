using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.Enums;
using MinTwitterApp.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace MinTwitterApp.Controllers;

public class AuthController : Controller
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    private readonly SessionService _sessionService;

    public AuthController(UserService userService, AuthService authService, SessionService sessionService)
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
    public async Task<IActionResult> Register(RegisterPageDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var errorCode = await _userService.RegisterAsync(model.Name, model.Email, model.Password);

        if (errorCode != UserRegisterErrorCode.None)
        {
            if (errorCode == UserRegisterErrorCode.EmailAlreadyExists)
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
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginPageDTO model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var user = await _authService.LoginAsync(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError("", "メールアドレスまたはパスワードが正しくありません。");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("MyCookieAuth", principal);

        _sessionService.SetUserId(user.Id);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }


        return RedirectToAction("Index", "CreatePost");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        _sessionService.Clear();

        TempData["Message"] = "ログアウトしました。";
        return RedirectToAction("Index", "Home");
    }

}