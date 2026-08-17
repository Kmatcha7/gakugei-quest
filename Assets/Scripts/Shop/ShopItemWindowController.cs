using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// お店画面のアイテムウィンドウを制御するクラスです。
    /// </summary>
    public class ShopItemWindowController : MonoBehaviour
    {
        /// <summary>
        /// お店画面全体を管理するクラスへの参照です。
        /// </summary>
        ShopManager _shopManager;

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopItemUIController _uiController;

        /// <summary>
        /// アイテム購入に関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopItemWindowBuyController _buyController;

        /// <summary>
        /// アイテム売却に関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopItemWindowSellController _sellController;

        /// <summary>
        /// お店で装備に関する情報表示のウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopEquipmentInformationWindowController _equipmentInformationWindowController;

        /// <summary>
        /// 現在選択中の項目のインデックスです。
        /// </summary>
        int _selectedIndex;

        /// <summary>
        /// 現在のページ数です。
        /// </summary>
        int _page;

        /// <summary>
        /// 項目選択が可能かどうかのフラグです。
        /// </summary>
        bool _canSelect;

        /// <summary>
        /// 1ページあたりのアイテム表示数です。
        /// </summary>
        readonly int ItemsInPage = 8;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(ShopManager shopManager)
        {
            _shopManager = shopManager;
        }

        /// <summary>
        /// ShopManagerへの参照を取得します。
        /// </summary>
        public ShopManager GetShopManager()
        {
            return _shopManager;
        }

        /// <summary>
        /// 購入に関する制御クラスへの参照を取得します。
        /// </summary>
        public ShopItemWindowBuyController GetBuyController()
        {
            return _buyController;
        }

        /// <summary>
        /// 売却に関する制御クラスへの参照を取得します。
        /// </summary>
        public ShopItemWindowSellController GetSellController()
        {
            return _sellController;
        }

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        void InitializeControllers()
        {
            _buyController.SetItemsInPage(ItemsInPage);
            _buyController.SetReferences(this);
            _buyController.InitializeItemInfo();

            _sellController.SetItemsInPage(ItemsInPage);
            _sellController.SetReferences(this);
            _sellController.InitializeItemInfo();
        }

        void Update()
        {
            SelectItem();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectItem()
        {
            if (_shopManager == null)
            {
                return;
            }

            if (!_canSelect)
            {
                return;
            }

            if (InputGameKey.MenuUp())
            {
                SelectUpperItem();
            }
            else if (InputGameKey.MenuDown())
            {
                SelectLowerItem();
            }
            else if (InputGameKey.MenuRight())
            {
                ShowNextPage();
            }
            else if (InputGameKey.MenuLeft())
            {
                ShowPrevPage();
            }
            else if (InputGameKey.ConfirmButton())
            {
                OnPressedConfirmButton();
            }
            else if (InputGameKey.CancelButton())
            {
                OnPressedCancelButton();
            }
        }

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        bool IsValidIndex(int index)
        {
            bool isValid = false;
            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                isValid = _buyController.IsValidIndex(index);
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                isValid = _sellController.IsValidIndex(index);
            }
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// </summary>
        bool IsValidSelection()
        {
            bool isValid = false;
            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                isValid = _buyController.IsValidSelection(_selectedIndex);
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                isValid = _sellController.IsValidSelection(_selectedIndex);
            }
            return isValid;
        }

        /// <summary>
        /// 次のページを表示します。
        /// </summary>
        void ShowNextPage()
        {
            // 最大ページ数が1以下の場合は何もしません。
            if (GetMaxPageNum() <= 1)
            {
                return;
            }

            _page++;

            // 次のページが存在しない場合は先頭のページに戻ります。
            if (_page >= GetMaxPageNum())
            {
                _page = 0;
            }

            // カーソルを先頭に移動します。
            _selectedIndex = _page * ItemsInPage;
            SetPageElement();

            PostSelection();
        }

        /// <summary>
        /// 前のページを表示します。
        /// </summary>
        void ShowPrevPage()
        {
            // 最大ページ数が1以下の場合は何もしません。
            if (GetMaxPageNum() <= 1)
            {
                return;
            }

            _page--;

            // 前のページが存在しない場合は最後尾のページに戻ります。
            if (_page < 0)
            {
                _page = GetMaxPageNum() - 1;
            }

            // カーソルを先頭に移動します。
            _selectedIndex = _page * ItemsInPage;
            SetPageElement();

            PostSelection();
        }

        /// <summary>
        /// ひとつ上の項目を選択します。
        /// </summary>
        void SelectUpperItem()
        {
            int newIndex = _selectedIndex - 1;
            int pageStartIndex = _page * ItemsInPage;
            if (newIndex < pageStartIndex)
            {
                int itemCount = GetPageItemCount();
                newIndex = _page * ItemsInPage + itemCount - 1;
            }

            if (IsValidIndex(newIndex))
            {
                _selectedIndex = newIndex;
            }

            PostSelection();
        }

        /// <summary>
        /// ひとつ下の項目を選択します。
        /// </summary>
        void SelectLowerItem()
        {
            int newIndex = _selectedIndex + 1;
            int itemCount = GetPageItemCount();
            int pageEndIndex = _page * ItemsInPage + itemCount - 1;
            if (newIndex > pageEndIndex)
            {
                newIndex = _page * ItemsInPage;
            }

            if (IsValidIndex(newIndex))
            {
                _selectedIndex = newIndex;
            }

            PostSelection();
        }

        /// <summary>
        /// ページ内の要素数を取得します。
        /// </summary>
        int GetPageItemCount()
        {
            int itemCount = 0;
            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                itemCount = _buyController.GetPageItemCount();
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                itemCount = _sellController.GetPageItemCount();
            }
            return itemCount;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        int GetMaxPageNum()
        {
            int maxPage = 0;
            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                maxPage = _buyController.GetMaxPageNum();
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                maxPage = _sellController.GetMaxPageNum();
            }
            return maxPage;
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void PostSelection()
        {
            ShowSelectionCursor();
            ShowSelectedItemDescription();
        }

        /// <summary>
        /// 選択中の位置が有効な範囲か確認します。
        /// </summary>
        void CheckSelectionPosition()
        {
            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                _selectedIndex = _buyController.VerifyIndex(_selectedIndex);
                _page = _buyController.VerifyPage(_page);
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                _selectedIndex = _sellController.VerifyIndex(_selectedIndex);
                _page = _sellController.VerifyPage(_page);
            }
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void ShowSelectionCursor()
        {
            int index = _selectedIndex % ItemsInPage;
            _uiController.ShowSelectedCursor(index);
        }

        /// <summary>
        /// 選択中の項目の説明を表示します。
        /// </summary>
        void ShowSelectedItemDescription()
        {
            int itemId = 0;
            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                _buyController.SetItemDescription(_selectedIndex, _uiController);
                var itemData = _buyController.GetItemData(_selectedIndex);
                if (itemData != null)
                {
                    itemId = itemData.itemId;
                }
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                _sellController.SetItemDescription(_selectedIndex, _uiController);
                var itemInfo = _sellController.GetItemInfo(_selectedIndex);
                if (itemInfo != null)
                {
                    itemId = itemInfo.itemId;
                }
            }
            SetEquipmentInformation(itemId);
        }

        /// <summary>
        /// 選択中の項目を装備した場合のステータス変化を表示します。
        /// </summary>
        void SetEquipmentInformation(int itemId)
        {
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
                newWeaponId = status.equipWeaponId;
                newArmorId = status.equipArmorId;

                var itemData = ItemDataManager.GetItemDataById(itemId);
                if (itemData == null)
                {
                    SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。itemId: {itemId}");
                }
                else
                {
                    if (itemData.itemCategory == ItemCategory.Weapon)
                    {
                        newWeaponId = itemId;
                        newArmorId = status.equipArmorId;
                    }
                    else if (itemData.itemCategory == ItemCategory.Armor)
                    {
                        newWeaponId = status.equipWeaponId;
                        newArmorId = itemId;
                    }
                }
            }
            _equipmentInformationWindowController.SetSelectedItemStatusValue(characterId, newWeaponId, newArmorId);
        }

        /// <summary>
        /// 現在のページ数テキストを表示します。
        /// </summary>
        void ShowPagerText()
        {
            int showPage = _page + 1;
            int maxPage = GetMaxPageNum();
            if (maxPage <= 0)
            {
                maxPage = 1;
            }
            string pageText = $"{showPage} / {maxPage}";
            _uiController.SetPagerText(pageText);
        }

        /// <summary>
        /// ウィンドウの状態をセットアップします。
        /// </summary>
        public void SetUpWindow()
        {
            _uiController.ClearAllItemText();
            _uiController.ClearDescriptionText();
            InitializeControllers();
            InitializeSelect();
            SetGoldValue();
        }

        /// <summary>
        /// 項目選択を初期化します。
        /// </summary>
        public void InitializeSelect()
        {
            _page = 0;
            _selectedIndex = 0;
            _uiController.ShowSelectedCursor(_selectedIndex);
            PostSelection();
        }

        /// <summary>
        /// ページ内の項目をセットします。
        /// </summary>
        public void SetPageElement()
        {
            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                _buyController.SetPageItem(_page, _uiController);
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                _sellController.SetPageItem(_page, _uiController);
            }

            // ページ送りのカーソルの表示状態を確認します。左右にループして表示するため、最大ページ数が1より大きい場合は表示するようにします。
            bool isVisiblePrevCursor = GetMaxPageNum() > 1;
            bool isVisibleNextCursor = GetMaxPageNum() > 1;
            _uiController.SetPrevCursorVisibility(isVisiblePrevCursor);
            _uiController.SetNextCursorVisibility(isVisibleNextCursor);
            ShowPagerText();
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            if (!IsValidSelection())
            {
                return;
            }

            if (_shopManager.SelectedCommand == ShopCommand.Buy)
            {
                var itemData = _buyController.GetItemData(_selectedIndex);
                if (itemData == null)
                {
                    return;
                }
                _shopManager.OnSelectedItem(itemData);

                // 選択時の効果音を再生します。
                AudioManager.Instance.PlaySe(SeNames.OK);
            }
            else if (_shopManager.SelectedCommand == ShopCommand.Sell)
            {
                var itemData = _sellController.GetItemData(_selectedIndex);
                if (itemData == null)
                {
                    return;
                }
                _shopManager.OnSelectedItem(itemData);

                // 選択時の効果音を再生します。
                AudioManager.Instance.PlaySe(SeNames.OK);
            }
            SetCanSelectState(false);
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        void OnPressedCancelButton()
        {
            // キャンセル時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Cancel);
            StartCoroutine(HideProcess());
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        IEnumerator HideProcess()
        {
            SetCanSelectState(false);
            yield return null;

            _shopManager.OnCanceled();
            HideWindow();
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// 選択ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }

        /// <summary>
        /// 項目選択が可能かどうかの状態を設定します。
        /// </summary>
        /// <param name="canSelect">項目選択が可能かどうか</param>
        public void SetCanSelectState(bool canSelect)
        {
            _canSelect = canSelect;
        }

        /// <summary>
        /// 所持金のテキストをセットします。
        /// </summary>
        void SetGoldValue()
        {
            int gold = CharacterStatusManager.partyGold;
            _uiController.SetGoldText(gold);
        }

        /// <summary>
        /// 購入または売却処理が終わった後の処理を行います。
        /// </summary>
        public void PostAction()
        {
            CheckSelectionPosition();
            SetPageElement();
            SetCanSelectState(false);
            HideWindow();
            _shopManager.PostAction();
        }
    }
}
