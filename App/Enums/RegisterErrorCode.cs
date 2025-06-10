namespace MinTwitterApp.Enums;

// 命名が微妙 UserErrorCodeだと？ UserRegisterErrorCodeにする？
public enum RegisterErrorCode
{
    None,
    NameEmpty,
    EmailEmpty,
    PasswordEmpty,
    EmailAlreadyExists,
    InvalidEmailFormat,
    UnknownError
}