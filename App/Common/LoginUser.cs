namespace MinTwitterApp.Common;

public class LoginUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LoginUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var loginUser = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(loginUser, out int userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("ログインユーザーが特定できません。");
    }

}
