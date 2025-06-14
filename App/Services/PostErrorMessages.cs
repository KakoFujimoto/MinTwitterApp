namespace MinTwitterApp.Services;

using MinTwitterApp.Enums;

public class PostErrorMessages : IPostErrorMessages
{
    private static readonly Dictionary<PostErrorCode, string> Messages = new()
    {
        // 投稿・編集用
        { PostErrorCode.ContentEmpty, "内容を入力してください。" },
        { PostErrorCode.InvalidImageExtension, "対応していない画像形式です。jpg, png, gif を使用してください。" },
        { PostErrorCode.InvalidImageFormat, "画像ファイルが正しくありません。" },
        { PostErrorCode.ImageReadError, "画像の読み込み中にエラーが発生しました。" },

        // 削除処理用
        { PostErrorCode.NotFound, "投稿が見つかりません"},
        { PostErrorCode.AlreadyDeleted, "この投稿は既に削除されています"}
    };

    public string GetErrorMessage(PostErrorCode errorCode)
    {
        return Messages.GetValueOrDefault(errorCode, "投稿に失敗しました。");
    }
}
