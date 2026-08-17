using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// セーブ枠を制御するクラスです。
    /// </summary>
    public class MenuSaveSlotController : MonoBehaviour
    {
        /// <summary>
        /// 項目のカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObj;

        /// <summary>
        /// ファイル名のテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _fileNameText;

        /// <summary>
        /// 先頭にいるキャラクターの名前テキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _characterNameText;

        /// <summary>
        /// レベルのタイトルテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _levelTitleText;

        /// <summary>
        /// レベルの値テキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _levelValueText;

        /// <summary>
        /// セーブした場所のテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _placeText;

        /// <summary>
        /// 項目が選択可能なときの色です。
        /// </summary>
        [SerializeField]
        Color _selectableColor = Color.white;

        /// <summary>
        /// 項目が選択不可能なときの色です。
        /// </summary>
        [SerializeField]
        Color _unselectableColor = Color.gray;

        /// <summary>
        /// ファイル名テキストをセットします。
        /// </summary>
        /// <param name="slotId">セーブ枠のID</param>
        public void SetFileNameText(int slotId)
        {
            string filePrefix = "ファイル ";
            _fileNameText.text = $"{filePrefix}{slotId}";
        }

        /// <summary>
        /// キャラクターの名前テキストをセットします。
        /// </summary>
        /// <param name="characterName">キャラクターの名前</param>
        public void SetCharacterNameText(string characterName)
        {
            _characterNameText.text = characterName;
        }

        /// <summary>
        /// レベルテキストをセットします。
        /// </summary>
        /// <param name="level">レベル</param>
        public void SetLevelText(int level)
        {
            _levelTitleText.text = "レベル";
            _levelValueText.text = level.ToString();
        }

        /// <summary>
        /// セーブした場所のテキストをセットします。
        /// </summary>
        /// <param name="place">セーブした場所</param>
        public void SetPlaceText(string place)
        {
            _placeText.text = place;
        }

        /// <summary>
        /// セーブ枠内の進行状況に関するテキストをクリアします。
        /// </summary>
        public void ClearSlotInfoText()
        {
            _characterNameText.text = string.Empty;
            _levelTitleText.text = string.Empty;
            _levelValueText.text = string.Empty;
            _placeText.text = string.Empty;
        }

        /// <summary>
        /// セーブ枠内のテキストの色をセットします。
        /// </summary>
        /// <param name="color">設定する色</param>
        public void SetTextColor(Color color)
        {
            _fileNameText.color = color;
            _characterNameText.color = color;
            _levelTitleText.color = color;
            _levelValueText.color = color;
            _placeText.color = color;
        }

        /// <summary>
        /// 項目が選択できる時のセーブ枠内のテキストの色をセットします。
        /// </summary>
        public void SetEnabledColor()
        {
            SetTextColor(_selectableColor);
        }

        /// <summary>
        /// 項目が選択できない時のセーブ枠内のテキストの色をセットします。
        /// </summary>
        public void SetDisabledColor()
        {
            SetTextColor(_unselectableColor);
        }

        /// <summary>
        /// 選択項目のカーソルを表示します。
        /// </summary>
        public void ShowCursor()
        {
            _cursorObj.SetActive(true);
        }

        /// <summary>
        /// 選択項目のカーソルを非表示にします。
        /// </summary>
        public void HideCursor()
        {
            _cursorObj.SetActive(false);
        }
    }
}