using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 選択肢ウィンドウのUIを制御するクラスです。
    /// </summary>
    public class OptionUIController : MonoBehaviour
    {
        /// <summary>
        /// 選択肢ウィンドウに存在する選択肢の一覧です。
        /// </summary>
        [SerializeField]
        List<OptionItemController> _optionItems = new List<OptionItemController>();

        /// <summary>
        /// 選択肢ウィンドウに存在する選択肢のカウントです。
        /// </summary>
        public int OptionCount => _optionItems.Count;

        /// <summary>
        /// カーソルを全て非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            foreach (var item in _optionItems)
            {
                item.HideCursor();
            }
        }

        /// <summary>
        /// 指定したインデックスの選択肢のカーソルを表示します。
        /// </summary>
        /// <param name="index">表示するカーソルのインデックス</param>
        public void ShowCursor(int index)
        {
            HideAllCursor();
            if (index >= 0 && index < _optionItems.Count)
            {
                _optionItems[index].ShowCursor();
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