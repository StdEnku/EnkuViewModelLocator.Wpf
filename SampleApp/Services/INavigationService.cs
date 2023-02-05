namespace SampleApp.Services;

using System;

/// <summary>
/// 画面遷移用ViewServiceのインターフェース
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 指定したUriへ遷移するためのメソッド
    /// </summary>
    /// <param name="uri">目的のコンテンツの URI で初期化された Uri オブジェクト。</param>
    /// <param name="extraData">ナビゲーション中の処理に使用するデータを格納している Object。</param>
    /// <returns>ナビゲーションがキャンセルされない場合は、true。それ以外の場合は false。</returns>
    bool Navigate(Uri uri, object? extraData = null);

    /// <summary>
    /// 進むナビゲーション履歴の最新の項目に移動するためのメソッド。
    /// </summary>
    void GoForward();

    /// <summary>
    /// 戻るナビゲーション履歴の最新の項目に移動するためのメソッド。
    /// </summary>
    void GoBack();
}