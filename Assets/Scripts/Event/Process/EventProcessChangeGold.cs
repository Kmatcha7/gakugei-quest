using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// ゴールドを増減させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessChangeGold : EventProcessBase
    {
        /// <summary>
        /// 変化させる対象のゴールドの量です。
        /// </summary>
        [SerializeField]
        int _goldAmount;

        /// <summary>
        /// 取得したゴールドのメッセージを生成するかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isGenerateMessage = true;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            CharacterStatusManager.IncreaseGold(_goldAmount);

            string message = string.Empty;
            if (_goldAmount >= 0)
            {
                message = $"{_goldAmount} ゴールド手に入れた！";
            }
            else
            {
                message = $"{-_goldAmount} ゴールド失った……。";
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