namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面のウィンドウを制御するクラス向けのインタフェースです。
    /// </summary>
    public interface IMenuWindowController
    {
        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        void SetUpController(MenuManager menuManager);

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        void ShowWindow();

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        void HideWindow();
    }
}