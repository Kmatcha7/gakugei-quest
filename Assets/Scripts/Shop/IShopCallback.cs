namespace SimpleRpg
{
    /// <summary>
    /// お店のコールバック用インタフェースです。
    /// </summary>
    public interface IShopCallback
    {
        /// <summary>
        /// お店の処理が終了したことを通知するコールバックです。
        /// </summary>
        void OnFinishedShopProcess();
    }
}