namespace SimpleRpg
{
    /// <summary>
    /// キャラクターの移動に関するインタフェースです。
    /// </summary>
    public interface ICharacterMoveCallback
    {
        /// <summary>
        /// キャラクターの移動が完了したことを通知するコールバックです。
        /// </summary>
        void OnFinishedMove();
    }
}