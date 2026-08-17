using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

namespace SimpleRpg
{
    /// <summary>
    /// メニューのゲーム終了画面のウィンドウを制御するクラスです。
    /// </summary>
    public class MenuQuitGameWindowController : MonoBehaviour, IMenuWindowController, IMessageCallback, IOptionCallback
    {
        /// <summary>
        /// メニュー画面に関する機能を管理するクラスへの参照です。
        /// </summary>
        MenuManager _menuManager;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MapMessageWindowController _messageWindowController;

        /// <summary>
        /// 選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        OptionWindowController _optionWindowController;

        /// <summary>
        /// ゲーム終了の確認メッセージです。
        /// </summary>
        readonly string QuitGameMessage = "ゲームを終了しますか？";

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        /// <summary>
        /// メッセージウィンドウを表示します。
        /// </summary>
        public void ShowMessageWindow()
        {
            _messageWindowController.SetUpController(this);
            _messageWindowController.ShowWindow();
            _messageWindowController.ShowGeneralMessage(QuitGameMessage, 0f);
        }

        /// <summary>
        /// 選択肢ウィンドウを表示します。
        /// </summary>
        public void ShowOptionWindow()
        {
            // デフォルトでいいえを選択している状態にします。
            int optionIndexNo = 1;
            _optionWindowController.SetUpController(this, optionIndexNo);
            _optionWindowController.ShowWindow();
        }

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            ShowMessageWindow();
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            StartCoroutine(HideWindowProcess());
        }

        /// <summary>
        /// 1フレーム後にウィンドウを非表示にします。
        /// </summary>
        IEnumerator HideWindowProcess()
        {
            yield return null;
            _messageWindowController.HideWindow();
            _optionWindowController.HideWindow();
            _menuManager.OnQuitCanceled();
        }

        /// <summary>
        /// メッセージの表示が完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            ShowOptionWindow();
        }

        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        public void OnSelectedOption(int selectedIndex)
        {
            if (selectedIndex == 0)
            {
#if UNITY_EDITOR
                // エディタ上ではプレイモードを終了します。
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // 実行環境ではアプリケーションを終了します。
                Application.Quit();
#endif
            }
            else
            {
                HideWindow();
            }
        }
    }
}
