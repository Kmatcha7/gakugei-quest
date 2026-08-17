using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 選択肢の項目を制御するクラスです。
    /// </summary>
    public class OptionItemController : MonoBehaviour
    {
        /// <summary>
        /// 選択肢のカーソルオブジェクトへの参照です。
        /// </summary>
        [SerializeField]
        GameObject _cursorObj;

        /// <summary>
        /// 選択肢のカーソルを表示します。
        /// </summary>
        public void ShowCursor()
        {
            _cursorObj.SetActive(true);
        }

        /// <summary>
        /// 選択肢のカーソルを非表示にします。
        /// </summary>
        public void HideCursor()
        {
            _cursorObj.SetActive(false);
        }
    }
}