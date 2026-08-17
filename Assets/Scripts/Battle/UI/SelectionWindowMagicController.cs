using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 選択ウィンドウにて魔法に関する処理を制御するクラスです。
    /// </summary>
    public class SelectionWindowMagicController : MonoBehaviour
    {
        /// <summary>
        /// 項目オブジェクトと魔法IDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _magicIdDictionary = new();

        /// <summary>
        /// キャラクターが覚えている魔法の一覧です。
        /// </summary>
        List<MagicData> _characterMagicList = new();

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        public bool IsValidIndex(int index)
        {
            bool isValid = index >= 0 && index < _characterMagicList.Count;
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        /// <param name="selectedIndex">選択中のインデックス</param>
        public bool IsValidSelection(int selectedIndex)
        {
            bool isValid = false;
            int indexInPage = selectedIndex % 4;

            // インデックスが辞書に存在しない場合は有効でないと判断します。
            if (!_magicIdDictionary.ContainsKey(indexInPage))
            {
                return isValid;
            }

            var magicId = _magicIdDictionary[indexInPage];
            var magicData = MagicDataManager.GetMagicDataById(magicId);
            isValid = CanSelectMagic(magicData);
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            int maxPage = Mathf.CeilToInt(_characterMagicList.Count * 1.0f / 4.0f);
            return maxPage;
        }

        /// <summary>
        /// キャラクターが覚えている魔法をリストにセットします。
        /// </summary>
        public void SetCharacterMagic()
        {
            _characterMagicList.Clear();

            // 指定したキャラクターのステータスを取得します。
            var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
            foreach (var magicId in characterStatus.magicList)
            {
                var magicData = MagicDataManager.GetMagicDataById(magicId);
                _characterMagicList.Add(magicData);
            }
        }

        /// <summary>
        /// ページ内の魔法の項目をセットします。
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetPageMagic(int page, SelectionUIController uiController)
        {
            _magicIdDictionary.Clear();
            int startIndex = page * 4;
            for (int i = startIndex; i < startIndex + 4; i++)
            {
                int positionIndex = i - startIndex;
                if (i < _characterMagicList.Count)
                {
                    var magicData = _characterMagicList[i];
                    string magicName = magicData.magicName;
                    int magicCost = magicData.cost;
                    bool canSelect = CanSelectMagic(magicData);
                    uiController.SetItemText(positionIndex, magicName, magicCost, canSelect);
                    uiController.SetDescriptionText(magicData.magicDesc);
                    _magicIdDictionary.Add(positionIndex, magicData.magicId);
                }
                else
                {
                    uiController.ClearItemText(positionIndex);
                }
            }

            if (_magicIdDictionary.Count == 0)
            {
                string noMagicText = "* 選択できる魔法がありません！ *";
                uiController.SetDescriptionText(noMagicText);
            }
        }

        /// <summary>
        /// 魔法を使えるか確認します。
        /// </summary>
        /// <param name="magicData">魔法データ</param>
        bool CanSelectMagic(MagicData magicData)
        {
            if (magicData == null)
            {
                return false;
            }

            var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
            return characterStatus.currentMp >= magicData.cost;
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public MagicData GetMagicData(int selectedIndex)
        {
            MagicData magicData = null;
            if (selectedIndex >= 0 && selectedIndex < _characterMagicList.Count)
            {
                magicData = _characterMagicList[selectedIndex];
            }
            
            return magicData;
        }
    }
}