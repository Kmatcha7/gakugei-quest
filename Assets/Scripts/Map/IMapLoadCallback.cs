namespace SimpleRpg
{
    /// <summary>
    /// マップ用Prefabのロードが完了したことを伝えるインタフェースです。
    /// </summary>
    public interface IMapLoadCallback
    {
        /// <summary>
        /// ロードが完了した時に呼ばれるコールバックです。
        /// </summary>
        void OnFinishedLoad();
    }
}