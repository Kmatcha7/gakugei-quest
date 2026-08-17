using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面のメニューのUIを制御するクラスです。
    /// </summary>
    public class TitleMenuUIController : MonoBehaviour
    {
        /// <summary>
        /// はじめからメニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjStart;

        /// <summary>
        /// つづきからメニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjContinue;

        /// <summary>
        /// ゲームの終了メニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjQuitGame;

        /// <summary>
        /// コマンドのカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _cursorObjStart.SetActive(false);
            _cursorObjContinue.SetActive(false);
            _cursorObjQuitGame.SetActive(false);
        }

        /// <summary>
        /// 選択中の項目のカーソルを表示します。
        /// </summary>
        public void ShowSelectedCursor(TitleCommand command)
        {
            HideAllCursor();

            switch (command)
            {
                case TitleCommand.Start:
                    _cursorObjStart.SetActive(true);
                    break;
                case TitleCommand.Continue:
                    _cursorObjContinue.SetActive(true);
                    break;
                case TitleCommand.Quit:
                    _cursorObjQuitGame.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// UIを表示します。
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}