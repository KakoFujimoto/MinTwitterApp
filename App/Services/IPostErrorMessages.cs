using MinTwitterApp.Enums;

namespace MinTwitterApp.Services;

public interface IPostErrorMessages
{
    string GetErrorMessage(PostErrorCode errorCode);
}