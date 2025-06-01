namespace MinTwitterApp.Enums;

public enum RegisterErrorCode
{
    None,
    EmailAlreadyExists,
    InvalidEmailFormat,
    PasswordTooWeak,
    UnknownError
}