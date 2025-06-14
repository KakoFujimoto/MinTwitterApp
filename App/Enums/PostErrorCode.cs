namespace MinTwitterApp.Enums;

public enum PostErrorCode
{
    // 投稿・編集用
    None,                     // エラーなし
    ContentEmpty,             // コンテンツが空
    InvalidImageExtension,    // 画像の拡張子が不正
    InvalidImageFormat,       // 画像フォーマットが不正
    ImageReadError,           // 画像の読み込みエラー

    // 削除処理用
    NotFound,                 // 投稿が存在しない
    AlreadyDeleted            // 投稿はすでに削除済み
}