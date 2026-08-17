using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// アイテムを増減させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessChangeItem : EventProcessBase
    {
        /// <summary>
        /// 変化させる対象のアイテムIDです。
        /// </summary>
        [SerializeField]
        int _itemId;

        /// <summary>
        /// 増減させる量です。
        /// </summary>
        [SerializeField]
        int _itemNum;

        /// <summary>
        /// 取得したアイテムのメッセージを生成するかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isGenerateMessage = true;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            var item = ItemDataManager.GetItemDataById(_itemId);
            if (item == null)
            {
                SimpleLogger.Instance.LogError($"アイテムID {_itemId} が存在しません。");
                CallNextProcess();
                return;
            }

            string message = string.Empty;
            if (_itemNum >= 0)
            {
                CharacterStatusManager.IncreaseItem(_itemId, _itemNum);
                message = $"{item.itemName} を {_itemNum} 個手に入れた！";
            }
            else
            {
                CharacterStatusManager.DecreaseItem(_itemId, -_itemNum);
                message = $"{item.itemName} を {-_itemNum} 個失った……。";
            }

            SendMessageToNextProcess(message);
            CallNextProcess();
        }

        /// <summary>
        /// 次のプロセスがメッセージ表示の場合に生成したメッセージをセットします。
        /// </summary>
        /// <param name="message">生成したメッセージ</param>
        void SendMessageToNextProcess(string message)
        {
            if (!_isGenerateMessage)
            {
                return;
            }

            var messageProcess = _nextProcess as EventProcessMessage;
            if (messageProcess != null)
            {
                var messages = new List<string>(){message};
                messageProcess.SetMessageFromProcess(messages);
            }
        }
    }
}