namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面のウィンドウ内のUIを制御するクラス向けのインタフェースです。
    /// </summary>
    public interface IMenuUIController
    {
        /// <summary>
        /// UIを表示します。
        /// </summary>
        void Show();

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        void Hide();
    }
}