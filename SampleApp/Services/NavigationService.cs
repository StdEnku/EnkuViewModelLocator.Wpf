using SampleApp;

namespace SampleApp.Services;

using System;
using System.Windows.Navigation;

/// <summary>
/// 画面遷移用ViewService
/// </summary>
public class NavigationService : INavigationService
{
    private NavigationWindow MainNavigationWindow => (NavigationWindow)App.Current.MainWindow;

    /// <summary>
    /// 指定したUriへ遷移するためのメソッド
    /// </summary>
    /// <param name="uri">目的のコンテンツの URI で初期化された Uri オブジェクト。</param>
    /// <param name="extraData">ナビゲーション中の処理に使用するデータを格納している Object。</param>
    /// <returns>ナビゲーションがキャンセルされない場合は、true。それ以外の場合は false。</returns>
    public bool Navigate(Uri uri, object? extraData = null)
    {
        var navigationWindow = MainNavigationWindow;

        if (extraData is null)
        {
            return navigationWindow.Navigate(uri);
        }
        else
        {
            return navigationWindow.Navigate(uri, extraData);
        }
    }

    /// <summary>
    /// 進むナビゲーション履歴の最新の項目に移動するためのメソッド。
    /// </summary>
    public void GoForward()
    {
        var navigationWindow = MainNavigationWindow;
        navigationWindow.GoForward();
    }

    /// <summary>
    /// 戻るナビゲーション履歴の最新の項目に移動するためのメソッド。
    /// </summary>
    public void GoBack()
    {
        var navigationWindow = MainNavigationWindow;
        navigationWindow.GoBack();
    }
}