using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// メニューの装備画面で装備するアイテムの選択画面を制御するクラスです。
    /// </summary>
    public class MenuEquipmentSelectionWindowController : MonoBehaviour, IMenuWindowController
    {
        /// <summary>
        /// メニュー画面に関する機能を管理するクラスへの参照です。
        /// </summary>
        MenuManager _menuManager;

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentSelectionUIController _uiController;

        /// <summary>
        /// 装備画面でアイテムに関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentSelectionItemController _itemController;

        /// <summary>
        /// メニューウィンドウで装備の処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuProcessorEquipment _menuProcessorEquipment;

        /// <summary>
        /// メニュー画面の装備ウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentWindowController _windowController;

        /// <summary>
        /// メニューの装備画面で情報表示のウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentInformationWindowController _informationWindowController;

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
        readonly int ItemsInPage = 7;

        /// <summary>
        /// 選択中のアイテムのカテゴリ名の定義です。
        /// </summary>
        readonly string CategoryWeapon = "武器";

        /// <summary>
        /// 選択中のアイテムのカテゴリ名の定義です。
        /// </summary>
        readonly string CategoryArmor = "防具";

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        /// <summary>
        /// ウィンドウの情報をセットアップします。
        /// </summary>
        /// <param name="windowController">装備画面のコントローラ</param>
        public void SetUpWindow(MenuEquipmentWindowController windowController)
        {
            _windowController = windowController;
            _informationWindowController = windowController.GetInformationWindowController();

            _uiController.ClearAllItemText();
            _informationWindowController.SetDescription(CharacterStatusManager.NoEquipmentId);
            _itemController.SetReferences(_windowController, this);
            _itemController.SetItemsInPage(ItemsInPage);
            _itemController.InitializeEquipmentItemInfo();
            _menuProcessorEquipment.SetReferences(_windowController, this);

            SetEquipmentInfo();
            InitializeSelect();
        }

        void Update()
        {
            SelectItem();
        }

        /// <summary>
        /// 装備するアイテムを選択します。
        /// </summary>
        void SelectItem()
        {
            if (_menuManager == null)
            {
                return;
            }

            if (_menuManager.MenuPhase != MenuPhase.Equipment)
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
            bool isValid = _itemController.IsValidIndex(index);
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        bool IsValidSelection()
        {
            bool isValid = _itemController.IsValidSelection(_selectedIndex);
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
            int itemCount = _itemController.GetPageItemCount();
            return itemCount;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        int GetMaxPageNum()
        {
            int maxPage = _itemController.GetMaxPageNum();
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
            _itemController.SetItemInformation(_selectedIndex);
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
        /// 装備中のアイテムの情報をセットします。
        /// </summary>
        public void SetEquipmentInfo()
        {
            int equipmentId = 0;
            string category = string.Empty;

            int characterId = CharacterStatusManager.partyCharacter[0];
            var status = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (status == null)
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
            }
            else
            {
                if (_windowController.SelectedParts == EquipmentParts.Weapon)
                {
                    equipmentId = status.equipWeaponId;
                    category = CategoryWeapon;
                }
                else if (_windowController.SelectedParts == EquipmentParts.Armor)
                {
                    equipmentId = status.equipArmorId;
                    category = CategoryArmor;
                }
            }

            string itemName = _windowController.GetItemName(equipmentId);
            _uiController.SetEquipmentItemNameText(itemName);
            _uiController.SetCategoryText(category);
        }

        /// <summary>
        /// ページ内の項目をセットします。
        /// </summary>
        public void SetPageElement()
        {
            _itemController.SetPageItem(_page, _uiController);

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

            var itemInfo = _itemController.GetItemInfo(_selectedIndex);
            if (itemInfo == null)
            {
                return;
            }

            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);
            _menuProcessorEquipment.EquipmentSelectedItem(itemInfo.itemId);
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

            _windowController.OnCanceledSelect();
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
        /// アイテムや魔法の使用処理が終わった後の処理です。
        /// </summary>
        public void PostAction()
        {
            _windowController.OnSelectedEquipmentItem();
        }
    }
}
