using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニューのトップ画面のUIを制御するクラスです。
    /// </summary>
    public class TopMenuUIController : MonoBehaviour, IMenuUIController
    {
        /// <summary>
        /// アイテムメニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjItem;

        /// <summary>
        /// 魔法メニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjMagic;

        /// <summary>
        /// 装備メニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjEquipment;

        /// <summary>
        /// ステータスメニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjStatus;

        /// <summary>
        /// セーブメニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjSave;

        /// <summary>
        /// ゲームの終了メニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjQuitGame;

        /// <summary>
        /// 閉じるメニューのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjClose;

        /// <summary>
        /// コマンドのカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _cursorObjItem.SetActive(false);
            _cursorObjMagic.SetActive(false);
            _cursorObjEquipment.SetActive(false);
            _cursorObjStatus.SetActive(false);
            _cursorObjSave.SetActive(false);
            _cursorObjQuitGame.SetActive(false);
            _cursorObjClose.SetActive(false);
        }

        /// <summary>
        /// 選択中の項目のカーソルを表示します。
        /// </summary>
        public void ShowSelectedCursor(MenuCommand command)
        {
            HideAllCursor();

            switch (command)
            {
                case MenuCommand.Item:
                    _cursorObjItem.SetActive(true);
                    break;
                case MenuCommand.Magic:
                    _cursorObjMagic.SetActive(true);
                    break;
                case MenuCommand.Equipment:
                    _cursorObjEquipment.SetActive(true);
                    break;
                case MenuCommand.Status:
                    _cursorObjStatus.SetActive(true);
                    break;
                case MenuCommand.Save:
                    _cursorObjSave.SetActive(true);
                    break;
                case MenuCommand.QuitGame:
                    _cursorObjQuitGame.SetActive(true);
                    break;
                case MenuCommand.Close:
                    _cursorObjClose.SetActive(true);
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