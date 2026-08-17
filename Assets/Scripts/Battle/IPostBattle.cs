namespace SimpleRpg
{
    /// <summary>
    /// 戦闘終了の通知を受け取るクラス向けのインタフェースです。
    /// </summary>
    public interface IPostBattle
    {
        /// <summary>
        /// 戦闘終了時のコールバックです。
        /// </summary>
        void OnFinishedBattle();

        /// <summary>
        /// 戦闘で負けた時のコールバックです。
        /// </summary>
        void OnLostBattle();
    }
}