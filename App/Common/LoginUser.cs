namespace MinTwitterApp.Common;

public class LoginUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LoginUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var loginUser = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return  loginUser??"";

    }

}
