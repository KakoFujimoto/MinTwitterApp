namespace MinTwitterApp.Services;

public interface ISessionService
{
    void SetUserId(int userId);
    int? GetUserId();
    void Clear();
}