using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 装備画面でアイテムに関する処理を制御するクラスです。
    /// </summary>
    public class MenuEquipmentSelectionItemController : MonoBehaviour
    {
        /// <summary>
        /// 装備画面のウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentWindowController _windowController;

        /// <summary>
        /// 装備画面の情報表示ウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentInformationWindowController _informationWindowController;

        /// <summary>
        /// 項目オブジェクトとアイテムIDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _itemIdDictionary = new();

        /// <summary>
        /// 1ページあたりのアイテム表示数です。
        /// </summary>
        int _itemInPage;

        /// <summary>
        /// 所持しているアイテムのうち、表示中の装備カテゴリに対応するアイテム情報のリストです。
        /// </summary>
        List<PartyItemInfo> _equipmentItemInfoList = new();

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メ装備画面のウィンドウを制御するクラス</param>
        /// <param name="selectionWindowController">メ装備画面のアイテム選択ウィンドウを制御するクラス</param>
        public void SetReferences(MenuEquipmentWindowController windowController, MenuEquipmentSelectionWindowController selectionWindowController)
        {
            _windowController = windowController;
            _informationWindowController = _windowController.GetInformationWindowController();
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
        /// 装備アイテム情報のリストをセットします。
        /// </summary>
        public void InitializeEquipmentItemInfo()
        {
            _equipmentItemInfoList.Clear();

            // 装備選択画面で表示する、装備を外すためのアイテムを追加します。
            PartyItemInfo noItemInfo = new()
            {
                itemId = CharacterStatusManager.NoEquipmentId,
                itemNum = 1
            };
            _equipmentItemInfoList.Add(noItemInfo);

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

                if (!IsMatchCategory(itemData))
                {
                    continue;
                }
                _equipmentItemInfoList.Add(partyItemInfo);
            }
        }

        /// <summary>
        /// 引数のアイテムが選択されたカテゴリに一致するか確認します。
        /// </summary>
        /// <param name="itemData">アイテムデータ</param>
        bool IsMatchCategory(ItemData itemData)
        {
            if (_windowController.SelectedParts == EquipmentParts.Weapon
                && IsWeapon(itemData))
            {
                return true;
            }
            else if (_windowController.SelectedParts == EquipmentParts.Armor
                && IsArmor(itemData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 引数のアイテムが武器か確認します。
        /// </summary>
        bool IsWeapon(ItemData itemData)
        {
            if (itemData == null)
            {
                return false;
            }
            return itemData.itemCategory == ItemCategory.Weapon;
        }

        /// <summary>
        /// 引数のアイテムが防具か確認します。
        /// </summary>
        bool IsArmor(ItemData itemData)
        {
            if (itemData == null)
            {
                return false;
            }
            return itemData.itemCategory == ItemCategory.Armor;
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
            bool isValid = index >= 0 && index < _equipmentItemInfoList.Count;
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
            if (!_itemIdDictionary.ContainsKey(indexInPage))
            {
                return isValid;
            }

            var itemId = _itemIdDictionary[indexInPage];
            var partyItemInfo = _equipmentItemInfoList.Find(info => info.itemId == itemId);
            isValid = partyItemInfo.itemNum > 0;
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            int maxPage = Mathf.CeilToInt(_equipmentItemInfoList.Count * 1.0f / _itemInPage * 1.0f);
            return maxPage;
        }

        /// <summary>
        /// ページ内のアイテムの項目をセットします。
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetPageItem(int page, MenuEquipmentSelectionUIController uiController)
        {
            _itemIdDictionary.Clear();
            int startIndex = page * _itemInPage;
            for (int i = startIndex; i < startIndex + _itemInPage; i++)
            {
                int positionIndex = i - startIndex;
                if (i < _equipmentItemInfoList.Count)
                {
                    var partyItemInfo = _equipmentItemInfoList[i];
                    if (partyItemInfo.itemId == CharacterStatusManager.NoEquipmentId)
                    {
                        uiController.SetItemText(positionIndex, partyItemInfo.itemId, _windowController.MissingItemName, 1, true);
                        _itemIdDictionary.Add(positionIndex, partyItemInfo.itemId);
                        continue;
                    }

                    var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
                    if (itemData == null)
                    {
                        SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {partyItemInfo.itemId}");
                        continue;
                    }
                    string itemName = itemData.itemName;
                    int itemNum = partyItemInfo.itemNum;
                    bool canSelect = CanSelectItem(partyItemInfo);
                    uiController.SetItemText(positionIndex, partyItemInfo.itemId, itemName, itemNum, canSelect);
                    _itemIdDictionary.Add(positionIndex, itemData.itemId);
                }
                else
                {
                    uiController.ClearItemText(positionIndex);
                }
            }

            if (_itemIdDictionary.Count == 0)
            {
                _informationWindowController.SetDescription(CharacterStatusManager.NoEquipmentId);
            }
        }

        /// <summary>
        /// 引数のインデックスに対応するアイテムの説明をセットします。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public void SetItemInformation(int selectedIndex)
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

            // 装備を外す項目を選択している場合は、アイテム定義の有無を確認しません。
            int itemId = partyItemInfo.itemId;
            if (itemId != CharacterStatusManager.NoEquipmentId)
            {
                var itemData = ItemDataManager.GetItemDataById(itemId);
                if (itemData == null)
                {
                    return;
                }
            }

            // カーソルの位置にあるアイテムの説明をセットします。
            _informationWindowController.SetDescription(itemId);

            // カーソルの位置にあるアイテムを装備した場合のステータスを表示します。
            int characterId = CharacterStatusManager.partyCharacter[0];
            int newWeaponId = 0;
            int newArmorId = 0;
            var status = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (status == null)
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
            }
            else
            {
                if (_windowController.SelectedParts == EquipmentParts.Weapon)
                {
                    newWeaponId = itemId;
                    newArmorId = status.equipArmorId;
                }
                else if (_windowController.SelectedParts == EquipmentParts.Armor)
                {
                    newWeaponId = status.equipWeaponId;
                    newArmorId = itemId;
                }
            }
            _informationWindowController.SetSelectedItemStatusValue(characterId, newWeaponId, newArmorId);
        }

        /// <summary>
        /// アイテムを装備できるか確認します。
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

            return IsMatchCategory(itemData);
        }

        /// <summary>
        /// 選択されたインデックスに対応するアイテム情報を取得します。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public PartyItemInfo GetItemInfo(int selectedIndex)
        {
            PartyItemInfo itemInfo = null;
            if (selectedIndex >= 0 && selectedIndex < _equipmentItemInfoList.Count)
            {
                itemInfo = _equipmentItemInfoList[selectedIndex];
            }
            return itemInfo;
        }
    }
}