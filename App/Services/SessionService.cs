namespace MinTwitterApp.Services;

public class SessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public void SetUserId(int userId)
    {
        _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", userId);
    }
    public int? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
    }

    public void Clear()
    {
        _httpContextAccessor.HttpContext?.Session.Clear();
    }
}