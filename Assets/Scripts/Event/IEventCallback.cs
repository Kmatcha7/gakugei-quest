namespace SimpleRpg
{
    /// <summary>
    /// イベントの処理が終了したことを伝えるインタフェースです。
    /// </summary>
    public interface IEventCallback
    {
        /// <summary>
        /// イベントが終了した時に呼ばれるコールバックです。
        /// </summary>
        void OnFinishedEvent();
    }
}