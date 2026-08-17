namespace SimpleRpg
{
    /// <summary>
    /// マップ上で表示するメッセージウィンドウを制御するクラスです。
    /// </summary>
    public class MapMessageWindowController : MessageWindowControllerBase
    {
        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="messageCallback">メッセージコールバック</param>
        public void SetUpController(IMessageCallback messageCallback)
        {
            _messageCallback = messageCallback;
        }

        /// <summary>
        /// 魔法を唱えた時のメッセージを生成します。
        /// </summary>
        public void GenerateMagicCastMessage(string magicUserName, string magicName)
        {
            uiController.ClearMessage();
            string message = $"{magicUserName}{BattleMessage.MagicUserSuffix} {magicName} {BattleMessage.MagicNameSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// HPが回復する時のメッセージを生成します。
        /// </summary>
        public void GenerateHpHealMessage(string targetName, int healNum)
        {
            string message = $"{targetName}{BattleMessage.HealTargetSuffix} {healNum} {BattleMessage.HealNumSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// アイテムを使用した時のメッセージを生成します。
        /// </summary>
        public void GenerateUseItemMessage(string itemUserName, string itemName)
        {
            uiController.ClearMessage();
            string message = $"{itemUserName}{BattleMessage.ItemUserSuffix} {itemName} {BattleMessage.ItemNameSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// 引数で指定されたメッセージを生成します。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="interval">メッセージ表示からコールバックまでの時間</param>
        public void ShowGeneralMessage(string message, float interval)
        {
            uiController.ClearMessage();
            StartCoroutine(ShowMessageAutoProcess(message, interval));
        }
    }
}