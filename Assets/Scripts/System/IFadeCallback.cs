namespace SimpleRpg
{
    /// <summary>
    /// 画面フェードのコールバック用インタフェースです。
    /// </summary>
    public interface IFadeCallback
    {
        /// <summary>
        /// フェードが完了したことを通知するコールバックです。
        /// </summary>
        void OnFinishedFade();
    }
}