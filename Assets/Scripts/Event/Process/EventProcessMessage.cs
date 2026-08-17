using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メッセージに関するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessMessage : EventProcessBase, IMessageCallback
    {
        /// <summary>
        /// このプロセスの終了後にメッセージウィンドウを開いたままにするかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _keepWindowOpen = false;

        /// <summary>
        /// 表示するメッセージです。
        /// 1つの要素につき1画面のメッセージを表示します。
        /// </summary>
        [SerializeField][TextArea]
        List<string> _messages;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MapMessageWindowController _messageWindowController;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            SetUpReference();
            if (_messageWindowController == null)
            {
                SimpleLogger.Instance.LogError("MapMessageWindowControllerが見つかりません。");
                CallNextProcess();
                return;
            }

            StartCoroutine(ShowMessageProcess());
        }

        /// <summary>
        /// メッセージのコントローラへの参照をセットします。
        /// </summary>
        void SetUpReference()
        {
            _messageWindowController = FindAnyObjectByType<MapMessageWindowController>();
        }

        /// <summary>
        /// メッセージを表示します。
        /// </summary>
        IEnumerator ShowMessageProcess()
        {
            _messageWindowController.SetUpController(this);
            _messageWindowController.ShowPager();
            _messageWindowController.ShowWindow();

            foreach (var message in _messages)
            {
                _messageWindowController.ShowGeneralMessage(message, 0f);

                // キー入力を待ちます。
                _messageWindowController.StartKeyWait();
                while (_messageWindowController.IsWaitingKeyInput)
                {
                    yield return null;
                }

                // 選択時の効果音を再生します。
                AudioManager.Instance.PlaySe(SeNames.OK);
            }

            if (!_keepWindowOpen)
            {
                _messageWindowController.HideWindow();
                _messageWindowController.HidePager();
            }

            CallNextProcess();
        }

        /// <summary>
        /// メッセージの表示が完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            
        }

        /// <summary>
        /// 別のプロセスからメッセージを設定します。
        /// </summary>
        public void SetMessageFromProcess(List<string> messages)
        {
            _messages = messages;
        }
    }
}