using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// アイテム売却に関する処理を制御するクラスです。
    /// </summary>
    public class ShopItemWindowSellController : MonoBehaviour
    {
        /// <summary>
        /// お店のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        ShopItemWindowController _windowController;

        /// <summary>
        /// 項目オブジェクトとアイテムIDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _itemIdDictionary = new();

        /// <summary>
        /// 1ページあたりのアイテム表示数です。
        /// </summary>
        int _itemInPage;

        /// <summary>
        /// 有効なアイテム情報のリストです。
        /// </summary>
        List<PartyItemInfo> _validPartyItemInfoList = new();

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(ShopItemWindowController windowController)
        {
            _windowController = windowController;
        }

        /// <summary>
        /// ページ内に表示するアイテム数をセットします。
        /// </summary>
        /// <param name="itemNum">ページ内のアイテム数</param>
        public void SetItemsInPage(int itemNum)
        {
            _itemInPage = itemNum;
        }

        /// <summary>
        /// 有効なアイテム情報のリストをセットします。
        /// </summary>
        public void InitializeItemInfo()
        {
            _validPartyItemInfoList.Clear();

            // アイテムデータに対応するIDがあるものをリストに追加します。
            foreach (var partyItemInfo in CharacterStatusManager.partyItemInfoList)
            {
                if (partyItemInfo == null)
                {
                    continue;
                }

                var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
                if (itemData == null)
                {
                    SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {partyItemInfo.itemId}");
                    continue;
                }
                _validPartyItemInfoList.Add(partyItemInfo);
            }
        }

        /// <summary>
        /// ページ内に存在するアイテム数を取得します。
        /// </summary>
        public int GetPageItemCount()
        {
            return _itemIdDictionary.Count;
        }

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        public bool IsValidIndex(int index)
        {
            bool isValid = index >= 0 && index < _validPartyItemInfoList.Count;
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// </summary>
        /// <param name="selectedIndex">選択中のインデックス</param>
        public bool IsValidSelection(int selectedIndex)
        {
            bool isValid = false;
            int indexInPage = selectedIndex % _itemInPage;
            if (!_itemIdDictionary.ContainsKey(indexInPage))
            {
                return isValid;
            }

            var itemId = _itemIdDictionary[indexInPage];
            var partyItemInfo = _validPartyItemInfoList.Find(info => info.itemId == itemId);
            isValid = partyItemInfo.itemNum > 0;
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            int maxPage = Mathf.CeilToInt(_validPartyItemInfoList.Count * 1.0f / _itemInPage * 1.0f);
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

            return _validPartyItemInfoList.Count - 1;
        }

        /// <summary>
        /// ページ内のアイテムの項目をセットします。
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetPageItem(int page, ShopItemUIController uiController)
        {
            _itemIdDictionary.Clear();
            int startIndex = page * _itemInPage;
            for (int i = startIndex; i < startIndex + _itemInPage; i++)
            {
                int positionIndex = i - startIndex;
                if (i < _validPartyItemInfoList.Count)
                {
                    SimpleLogger.Instance.Log($"SetPageItemの処理中: i={i}, positionIndex={positionIndex}, itemCount={_validPartyItemInfoList.Count}");
                    var partyItemInfo = _validPartyItemInfoList[i];
                    var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
                    if (itemData == null)
                    {
                        SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {partyItemInfo.itemId}");
                        continue;
                    }
                    string itemName = itemData.itemName;
                    int itemPrice = (int) (itemData.price * ValueSettings.SellPriceMultiplier);
                    bool canSelect = CanSelectItem(partyItemInfo);
                    uiController.SetItemText(positionIndex, itemName, itemPrice, canSelect);
                    _itemIdDictionary.Add(positionIndex, itemData.itemId);
                }
                else
                {
                    uiController.ClearItemText(positionIndex);
                }
            }

            if (_itemIdDictionary.Count == 0)
            {
                uiController.ClearDescriptionText();
            }
        }

        /// <summary>
        /// 引数のインデックスに対応するアイテムの説明をセットします。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetItemDescription(int selectedIndex, ShopItemUIController uiController)
        {
            if (!IsValidIndex(selectedIndex))
            {
                return;
            }

            var partyItemInfo = GetItemInfo(selectedIndex);
            if (partyItemInfo == null)
            {
                return;
            }

            int itemId = partyItemInfo.itemId;
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData != null)
            {
                uiController.SetDescriptionText(itemData.itemDesc);
            }
        }

        /// <summary>
        /// アイテムを売却できるか確認します。
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        bool CanSelectItem(PartyItemInfo partyItemInfo)
        {
            if (partyItemInfo == null)
            {
                return false;
            }

            var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
            if (itemData == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 選択されたインデックスに対応するアイテム情報を取得します。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public PartyItemInfo GetItemInfo(int selectedIndex)
        {
            PartyItemInfo itemInfo = null;
            if (selectedIndex >= 0 && selectedIndex < _validPartyItemInfoList.Count)
            {
                itemInfo = _validPartyItemInfoList[selectedIndex];
            }
            return itemInfo;
        }

        /// <summary>
        /// 選択されたインデックスに対応するアイテムデータを取得します。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public ItemData GetItemData(int selectedIndex)
        {
            ItemData itemData = null;
            PartyItemInfo itemInfo = GetItemInfo(selectedIndex);
            if (itemInfo != null)
            {
                itemData = ItemDataManager.GetItemDataById(itemInfo.itemId);
            }
            return itemData;
        }

        /// <summary>
        /// アイテムの売却処理が終わった時のコールバックです。
        /// </summary>
        public void OnFinishedItemProcess()
        {
            InitializeItemInfo();
            _windowController.PostAction();
        }
    }
}