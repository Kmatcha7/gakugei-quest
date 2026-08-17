using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// メニューの装備画面で装備するアイテムの選択画面のUIを制御するクラスです。
    /// </summary>
    public class MenuEquipmentSelectionUIController : MonoBehaviour, IMenuUIController
    {
        /// <summary>
        /// 選択中の装備カテゴリのテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _categoryText;

        /// <summary>
        /// 装備中のアイテム名のテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _equipmentItemNameText;

        /// <summary>
        /// 項目のコントローラへの参照用リストです。
        /// </summary>
        [SerializeField]
        List<SelectionItemController> _itemControllers;

        /// <summary>
        /// 前のページがあることを示すカーソルのオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjPrev;

        /// <summary>
        /// 次のページがあることを示すカーソルのオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjNext;

        /// <summary>
        /// ページ数を表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _pageNumText;

        /// <summary>
        /// 項目のカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            foreach (var controller in _itemControllers)
            {
                controller.HideCursor();
            }
        }

        /// <summary>
        /// 引数のインデックスが有効な範囲かどうかを判定します。
        /// </summary>
        /// <param name="index">インデックス</param>
        bool IsValidIndex(int index)
        {
            return index >= 0 && index < _itemControllers.Count;
        }

        /// <summary>
        /// 選択中の項目のカーソルを表示します。
        /// </summary>
        /// <param name="selectedPosition">選択中の位置</param>
        public void ShowSelectedCursor(int selectedPosition)
        {
            if (!IsValidIndex(selectedPosition))
            {
                return;
            }

            HideAllCursor();
            var selectedController = _itemControllers[selectedPosition];
            if (selectedController != null)
            {
                selectedController.ShowCursor();
            }
        }

        /// <summary>
        /// 選択中の装備カテゴリのテキストをセットします。
        /// </summary>
        public void SetCategoryText(string categoryText)
        {
            _categoryText.text = categoryText;
        }

        /// <summary>
        /// 現在装備中のアイテム名をセットします。
        /// </summary>
        public void SetEquipmentItemNameText(string itemNameText)
        {
            _equipmentItemNameText.text = itemNameText;
        }

        /// <summary>
        /// 指定した位置の項目のテキストをセットします。
        /// </summary>
        /// <param name="selectedPosition">位置</param>
        /// <param name="itemId">アイテムID</param>
        /// <param name="itemName">項目名</param>
        /// <param name="itemNum">項目数</param>
        /// <param name="canSelect">選択可能かどうか</param>
        public void SetItemText(int selectedPosition, int itemId, string itemName, int itemNum, bool canSelect)
        {
            if (!IsValidIndex(selectedPosition))
            {
                return;
            }

            var selectedController = _itemControllers[selectedPosition];
            if (selectedController != null)
            {
                if (itemId == CharacterStatusManager.NoEquipmentId)
                {
                    selectedController.SetItemText(itemName, string.Empty);
                }
                else
                {
                    selectedController.SetItemText(itemName, itemNum);
                }
                selectedController.SetItemTextColors(canSelect);
            }
        }

        /// <summary>
        /// 指定した位置の項目のテキストを初期化します。
        /// </summary>
        /// <param name="selectedPosition">位置</param>
        public void ClearItemText(int selectedPosition)
        {
            if (!IsValidIndex(selectedPosition))
            {
                return;
            }

            var selectedController = _itemControllers[selectedPosition];
            if (selectedController != null)
            {
                selectedController.ClearItemText();
            }
        }

        /// <summary>
        /// 全ての項目のテキストを初期化します。
        /// </summary>
        public void ClearAllItemText()
        {
            foreach (var controller in _itemControllers)
            {
                controller.ClearItemText();
            }
        }

        /// <summary>
        /// ページ表示テキストをセットします。
        /// </summary>
        public void SetPagerText(string pagerText)
        {
            _pageNumText.text = pagerText;
        }

        /// <summary>
        /// 前のページがあることを示すカーソルを表示します。
        /// </summary>
        /// <param name="isVisible">表示するかどうか</param>
        public void SetPrevCursorVisibility(bool isVisible)
        {
            _cursorObjPrev.SetActive(isVisible);
        }

        /// <summary>
        /// 次のページがあることを示すカーソルを表示します。
        /// </summary>
        /// <param name="isVisible">表示するかどうか</param>
        public void SetNextCursorVisibility(bool isVisible)
        {
            _cursorObjNext.SetActive(isVisible);
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