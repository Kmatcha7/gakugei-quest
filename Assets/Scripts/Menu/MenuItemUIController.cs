using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// メニューのアイテムウィンドウのUIを制御するクラスです。
    /// </summary>
    public class MenuItemUIController : MonoBehaviour, IMenuUIController
    {
        /// <summary>
        /// 説明テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _descriptionText;

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
        /// 指定した位置の項目のテキストをセットします。
        /// </summary>
        /// <param name="selectedPosition">位置</param>
        /// <param name="itemName">項目名</param>
        /// <param name="itemNum">項目数</param>
        /// <param name="canSelect">選択可能かどうか</param>
        public void SetItemText(int selectedPosition, string itemName, int itemNum, bool canSelect)
        {
            if (!IsValidIndex(selectedPosition))
            {
                return;
            }

            var selectedController = _itemControllers[selectedPosition];
            if (selectedController != null)
            {
                selectedController.SetItemText(itemName, itemNum);
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
        /// 説明テキストをセットします。
        /// </summary>
        /// <param name="description">説明</param>
        public void SetDescriptionText(string description)
        {
            _descriptionText.text = description;
        }

        /// <summary>
        /// 説明テキストを初期化します。
        /// </summary>
        public void ClearDescriptionText()
        {
            _descriptionText.text = string.Empty;
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