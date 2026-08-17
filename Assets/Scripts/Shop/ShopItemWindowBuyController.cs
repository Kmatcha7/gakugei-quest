using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// アイテム購入に関する処理を制御するクラスです。
    /// </summary>
    public class ShopItemWindowBuyController : MonoBehaviour
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
        List<ItemData> _validShopItemInfoList = new();

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
            _validShopItemInfoList.Clear();

            // アイテムデータに対応するIDがあるものをリストに追加します。
            var shopManager = _windowController.GetShopManager();
            foreach (var shopItemId in shopManager.ShopItems)
            {
                var itemData = ItemDataManager.GetItemDataById(shopItemId);
                if (itemData == null)
                {
                    SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {shopItemId}");
                    continue;
                }
                _validShopItemInfoList.Add(itemData);
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
            bool isValid = index >= 0 && index < _validShopItemInfoList.Count;
            return isValid;
        }

        /// <summary>
        /// 選択中の項目を購入できるか確認します。
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
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {itemId}");
                return isValid;
            }
            isValid = CanSelectItem(itemData);
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            int maxPage = Mathf.CeilToInt(_validShopItemInfoList.Count * 1.0f / _itemInPage * 1.0f);
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

            return _validShopItemInfoList.Count - 1;
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
                if (i < _validShopItemInfoList.Count)
                {
                    var itemData = _validShopItemInfoList[i];
                    string itemName = itemData.itemName;
                    int itemPrice = itemData.price;
                    bool canSelect = CanSelectItem(itemData);
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

            var itemData = _validShopItemInfoList[selectedIndex];
            if (itemData != null)
            {
                uiController.SetDescriptionText(itemData.itemDesc);
            }
        }

        /// <summary>
        /// アイテムを購入できるか確認します。
        /// </summary>
        /// <param name="itemData">購入するアイテム</param>
        bool CanSelectItem(ItemData itemData)
        {
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning("アイテムデータがnullです。");
                return false;
            }

            bool isValidGold = CharacterStatusManager.partyGold >= itemData.price;
            if (!isValidGold)
            {
                SimpleLogger.Instance.Log($"所持金が足りません。必要金額: {itemData.price}, 所持金: {CharacterStatusManager.partyGold}");
                return false;
            }

            bool hasItemSpace = true;
            var partyItemInfo = CharacterStatusManager.partyItemInfoList.Find(item => item.itemId == itemData.itemId);
            if (partyItemInfo != null)
            {
                hasItemSpace = partyItemInfo.itemNum < ValueSettings.MaxItemNum;
            }
            return hasItemSpace;
        }

        /// <summary>
        /// 選択されたインデックスに対応するアイテムデータを取得します。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public ItemData GetItemData(int selectedIndex)
        {
            ItemData itemData = null;
            if (selectedIndex >= 0 && selectedIndex < _validShopItemInfoList.Count)
            {
                itemData = _validShopItemInfoList[selectedIndex];
            }
            return itemData;
        }

        /// <summary>
        /// アイテムの購入処理が終わった時のコールバックです。
        /// </summary>
        public void OnFinishedItemProcess()
        {
            InitializeItemInfo();
            _windowController.PostAction();
        }
    }
}