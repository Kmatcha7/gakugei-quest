using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// メニューのセーブ画面のUIを制御するクラスです。
    /// </summary>
    public class MenuSaveUIController : MonoBehaviour, IMenuUIController
    {
        /// <summary>
        /// セーブ画面の説明テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _saveDescription;

        /// <summary>
        /// セーブ枠1のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        MenuSaveSlotController _saveSlotController1;

        /// <summary>
        /// セーブ枠2のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        MenuSaveSlotController _saveSlotController2;

        /// <summary>
        /// セーブ枠3のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        MenuSaveSlotController _saveSlotController3;

        /// <summary>
        /// セーブ枠のコントローラのリストです。
        /// </summary>
        List<MenuSaveSlotController> _saveSlotControllers = new();

        /// <summary>
        /// セーブ画面の説明テキストを設定します。
        /// </summary>
        /// <param name="description">セーブ画面の説明テキスト</param>
        public void SetDescription(string description)
        {
            _saveDescription.text = description;
        }

        /// <summary>
        /// セーブ枠のコントローラのリストを初期化します。
        /// </summary>
        public void SetUpControllerList()
        {
            _saveSlotControllers.Clear();
            _saveSlotControllers.Add(_saveSlotController1);
            _saveSlotControllers.Add(_saveSlotController2);
            _saveSlotControllers.Add(_saveSlotController3);
        }

        /// <summary>
        /// 全てのセーブ枠の情報をクリアします。
        /// </summary>
        public void ClearAllSlot()
        {
            foreach (var controller in _saveSlotControllers)
            {
                controller.ClearSlotInfoText();
            }
        }

        /// <summary>
        /// セーブ枠に表示する情報をセットします。
        /// </summary>
        /// <param name="slotId">セーブ枠のID</param>
        /// <param name="characterName">キャラクターの名前</param>
        /// <param name="level">レベル</param>
        /// <param name="place">セーブした場所</param>
        public void SetSlotInfo(int slotId, string characterName, int level, string place)
        {
            var controller = GetSlotController(slotId);
            if (controller == null)
            {
                return;
            }

            controller.SetFileNameText(slotId);
            controller.SetCharacterNameText(characterName);
            controller.SetLevelText(level);
            controller.SetPlaceText(place);
            controller.SetEnabledColor();
        }

        /// <summary>
        /// 空のセーブ枠に表示する情報をセットします。
        /// </summary>
        /// <param name="slotId">セーブ枠のID</param>
        /// <param name="emptyName">空欄時の名前</param>
        public void SetSlotInfoAsEmpty(int slotId, string emptyName)
        {
            var controller = GetSlotController(slotId);
            if (controller == null)
            {
                return;
            }

            controller.ClearSlotInfoText();
            controller.SetFileNameText(slotId);
            controller.SetCharacterNameText(emptyName);
            controller.SetDisabledColor();
        }

        /// <summary>
        /// 選択されたセーブ枠のカーソルを表示します。
        /// </summary>
        /// <param name="slotId">セーブ枠のID</param>
        public void ShowCursor(int slotId)
        {
            HideAllCursor();
            var controller = GetSlotController(slotId);
            if (controller == null)
            {
                return;
            }

            controller.ShowCursor();
        }

        /// <summary>
        /// 選択されたセーブ枠のカーソルを表示します。
        /// </summary>
        void HideAllCursor()
        {
            foreach (var controller in _saveSlotControllers)
            {
                controller.HideCursor();
            }
        }

        /// <summary>
        /// セーブ枠のIDに対応するコントローラを取得します。
        /// </summary>
        /// <param name="slotId">セーブ枠のID</param>
        MenuSaveSlotController GetSlotController(int slotId)
        {
            MenuSaveSlotController slotController = null;

            switch (slotId)
            {
                case 1:
                    slotController = _saveSlotController1;
                    break;
                case 2:
                    slotController = _saveSlotController2;
                    break;
                case 3:
                    slotController = _saveSlotController3;
                    break;
                default:
                    SimpleLogger.Instance.LogError($"存在しないセーブ枠のIDです。 ID: {slotId}");
                    break;
            }

            return slotController;
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