namespace MinTwitterApp.Services;

using MinTwitterApp.Enums;

public class PostErrorMessages : IPostErrorMessages
{
    private static readonly Dictionary<PostErrorCode, string> Messages = new()
    {
        { PostErrorCode.ContentEmpty, "内容を入力してください。" },
        { PostErrorCode.InvalidImageExtension, "対応していない画像形式です。jpg, png, gif を使用してください。" },
        { PostErrorCode.InvalidImageFormat, "画像ファイルが正しくありません。" },
        { PostErrorCode.ImageReadError, "画像の読み込み中にエラーが発生しました。" }
    };

    public string GetErrorMessage(PostErrorCode errorCode)
    {
        return Messages.GetValueOrDefault(errorCode, "投稿に失敗しました。");
    }
}
