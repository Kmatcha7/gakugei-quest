namespace SimpleRpg
{
    /// <summary>
    /// パーティ内のキャラクターを全回復させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessRefresh : EventProcessBase
    {
        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            CharacterStatusManager.RefreshPartyCharacter();
            CallNextProcess();
        }
    }
}