namespace SimpleRpg
{
    /// <summary>
    /// メッセージウィンドウのコールバック用インタフェースです。
    /// </summary>
    public interface IMessageCallback
    {
        /// <summary>
        /// メッセージの表示が完了したことを通知するコールバックです。
        /// </summary>
        void OnFinishedShowMessage();
    }
}