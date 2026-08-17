using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// メニューウィンドウにて魔法に関する処理を制御するクラスです。
    /// </summary>
    public class MenuItemWindowMagicController : MonoBehaviour
    {
        /// <summary>
        /// メニュー画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuItemWindowController _windowController;

        /// <summary>
        /// 項目オブジェクトと魔法IDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _magicIdDictionary = new();

        /// <summary>
        /// キャラクターが覚えている魔法の一覧です。
        /// </summary>
        List<MagicData> _characterMagicList = new();

        /// <summary>
        /// 1ページあたりの魔法表示数です。
        /// </summary>
        int _itemInPage;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(MenuItemWindowController windowController)
        {
            _windowController = windowController;
        }

        /// <summary>
        /// ページ内に表示する魔法数をセットします。
        /// </summary>
        /// <param name="itemNum">ページ内の魔法数</param>
        public void SetItemsInPage(int itemNum)
        {
            _itemInPage = itemNum;
        }

        /// <summary>
        /// ページ内に存在する魔法数を取得します。
        /// </summary>
        public int GetPageItemCount()
        {
            return _magicIdDictionary.Count;
        }

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
            int indexInPage = selectedIndex % _itemInPage;
            if (!_magicIdDictionary.ContainsKey(indexInPage))
            {
                return isValid;
            }

            var magicId = _magicIdDictionary[indexInPage];
            var magicData = _characterMagicList.Find(m => m.magicId == magicId);
            if (magicData == null)
            {
                return isValid;
            }

            int characterId = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            isValid = characterStatus.currentMp >= magicData.cost;
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            int maxPage = Mathf.CeilToInt(_characterMagicList.Count * 1.0f / _itemInPage * 1.0f);
            return maxPage;
        }

        /// <summary>
        /// 現在のページが有効な範囲か確認します。
        /// </summary>
        /// <param name="currentPage">現在のページ</param>
        public int VerifyPage(int currentPage)
        {
            if (currentPage >= 0 && currentPage < GetMaxPageNum())
            {
                return currentPage;
            }
            else if (currentPage <= 0)
            {
                return 0;
            }
            else
            {
                return GetMaxPageNum() - 1;
            }
        }

        /// <summary>
        /// 選択中のインデックスを有効な範囲に補正します。
        /// </summary>
        /// <param name="index">補正するインデックス</param>
        public int VerifyIndex(int index)
        {
            if (IsValidIndex(index))
            {
                return index;
            }

            return _characterMagicList.Count - 1;
        }

        /// <summary>
        /// キャラクターが覚えている魔法をリストにセットします。
        /// </summary>
        public void SetCharacterMagic()
        {
             _characterMagicList.Clear();

            foreach (var magicData in MagicDataManager.GetAllData())
            {
                if (magicData == null)
                    continue;

                // フラグ未設定なら常に使える
                if (string.IsNullOrEmpty(magicData.unlockFlagName))
                {
                    _characterMagicList.Add(magicData);
                    continue;
                }

                // フラグが立っていれば使える
                bool isUnlocked =FlagManager.Instance.GetFlagState(magicData.unlockFlagName);
                if (isUnlocked)
                {
                    _characterMagicList.Add(magicData);
                }
            }
        }

        /// <summary>
        /// ページ内の魔法の項目をセットします。
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetPageItem(int page, MenuItemUIController uiController)
        {
            _magicIdDictionary.Clear();
            int startIndex = page * _itemInPage;
            for (int i = startIndex; i < startIndex + _itemInPage; i++)
            {
                int positionIndex = i - startIndex;
                if (i < _characterMagicList.Count)
                {
                    var magicData = _characterMagicList[i];
                    if (magicData == null)
                    {
                        SimpleLogger.Instance.LogWarning($"魔法データが見つかりませんでした。");
                        continue;
                    }
                    string magicName = magicData.magicName;
                    int magicCost = magicData.cost;
                    bool canSelect = CanSelectMagic(magicData);
                    uiController.SetItemText(positionIndex, magicName, magicCost, canSelect);
                    _magicIdDictionary.Add(positionIndex, magicData.magicId);
                }
                else
                {
                    uiController.ClearItemText(positionIndex);
                }
            }

            if (_magicIdDictionary.Count == 0)
            {
                uiController.ClearDescriptionText();
            }
        }

        /// <summary>
        /// 引数のインデックスに対応する魔法の説明をセットします。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetMagicDescription(int selectedIndex, MenuItemUIController uiController)
        {
            if (!IsValidIndex(selectedIndex))
            {
                return;
            }

            var magicData = GetMagicData(selectedIndex);
            if (magicData == null)
            {
                return;
            }

            uiController.SetDescriptionText(magicData.magicDesc);
        }

        /// <summary>
        /// 魔法を使えるか確認します。
        /// </summary>
        /// <param name="magicData">魔法データ</param>
        public bool CanSelectMagic(MagicData magicData)
        {
            if (magicData == null)
            {
                return false;
            }

            // マップ上で使用できないカテゴリが含まれている場合はfalseを返します。
            bool hasUnusable = HasUnusableCategory(magicData);
            if (hasUnusable)
            {
                return false;
            }

            // 魔法の効果範囲に敵キャラクターが含まれている場合はfalseを返します。
            bool hasUnusableTarget = HasUnusableTarget(magicData);
            if (hasUnusableTarget)
            {
                return false;
            }

            // 消費MPが足りているか確認します。
            var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
            bool isValidCost = characterStatus.currentMp >= magicData.cost;
            return isValidCost;
        }

        /// <summary>
        /// 魔法効果にマップ上で使用できないカテゴリが含まれているか確認します。
        /// </summary>
        /// <param name="magicData">魔法データ</param>
        bool HasUnusableCategory(MagicData magicData)
        {
            bool hasUnusable = false;
            foreach (var effect in magicData.magicEffects)
            {
                // ダメージ系、サポート系、None系の効果はマップ上で使用できないようにします。
                if (effect.magicCategory == MagicCategory.Damage
                    || effect.magicCategory == MagicCategory.None
                    || effect.magicCategory == MagicCategory.Support)
                {
                    hasUnusable = true;
                    break;
                }
            }
            return hasUnusable;
        }

        /// <summary>
        /// 魔法の効果範囲に敵が含まれているか確認します。
        /// </summary>
        /// <param name="magicData">魔法データ</param>
        bool HasUnusableTarget(MagicData magicData)
        {
            bool hasUnusable = false;
            foreach (var effect in magicData.magicEffects)
            {
                // 敵キャラクターが対象の場合はマップ上で使用できないようにします。
                if (effect.effectTarget == EffectTarget.EnemySolo
                    || effect.effectTarget == EffectTarget.EnemyAll)
                {
                    hasUnusable = true;
                    break;
                }
            }
            return hasUnusable;
        }

        /// <summary>
        /// 選択されたインデックスに対応する魔法データを取得します。
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

        /// <summary>
        /// 魔法の使用処理が終わった時のコールバックです。
        /// </summary>
        public void OnFinishedMagicProcess()
        {
            _windowController.PostAction();
        }
    }
}