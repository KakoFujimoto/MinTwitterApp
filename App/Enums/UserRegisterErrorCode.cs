namespace MinTwitterApp.Enums;

public enum UserRegisterErrorCode
{
    None,
    NameEmpty,
    EmailEmpty,
    PasswordEmpty,
    EmailAlreadyExists,
    InvalidEmailFormat,
    UnknownError
}