using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メッセージウィンドウの動作を制御するクラスのベースクラスです。
    /// </summary>
    public class MessageWindowControllerBase : MonoBehaviour
    {
        /// <summary>
        /// メッセージウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        protected MessageUIController uiController;

        /// <summary>
        /// メッセージの表示間隔です。
        /// </summary>
        [SerializeField]
        protected float _messageInterval = 1.0f;

        /// <summary>
        /// メッセージの表示が完了した時のコールバック先です。
        /// </summary>
        protected IMessageCallback _messageCallback;

        /// <summary>
        /// メッセージのキー入力を待つかどうかのフラグです。
        /// </summary>
        public bool IsWaitingKeyInput { get; private set; }

        void Update()
        {
            KeyWait();
        }

        /// <summary>
        /// キー入力を待つ処理です。
        /// </summary>
        public void KeyWait()
        {
            if (!IsWaitingKeyInput)
            {
                return;
            }

            if (InputGameKey.ConfirmButton() || InputGameKey.CancelButton())
            {
                IsWaitingKeyInput = false;
                HidePager();
            }
        }

        /// <summary>
        /// キーの入力待ちを開始します。
        /// </summary>
        public void StartKeyWait()
        {
            IsWaitingKeyInput = true;
            ShowPager();
        }

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public virtual void ShowWindow()
        {
            uiController.ClearMessage();
            uiController.Show();
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public virtual void HideWindow()
        {
            uiController.ClearMessage();
            uiController.Hide();
        }

        /// <summary>
        /// ページ送りのカーソルを表示します。
        /// </summary>
        public virtual void ShowPager()
        {
            uiController.ShowCursor();
        }

        /// <summary>
        /// ページ送りのカーソルを非表示にします。
        /// </summary>
        public virtual void HidePager()
        {
            uiController.HideCursor();
        }

        /// <summary>
        /// メッセージを順番に表示するコルーチンです。
        /// </summary>
        protected IEnumerator ShowMessageAutoProcess(string message)
        {
            uiController.AppendMessage(message);
            yield return new WaitForSeconds(_messageInterval);
            _messageCallback.OnFinishedShowMessage();
        }

        /// <summary>
        /// メッセージを順番に表示するコルーチンです。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="interval">表示間隔</param>
        protected IEnumerator ShowMessageAutoProcess(string message, float interval)
        {
            uiController.AppendMessage(message);
            yield return new WaitForSeconds(interval);
            _messageCallback.OnFinishedShowMessage();
        }
    }
}