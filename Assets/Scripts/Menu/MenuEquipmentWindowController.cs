using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面の装備ウィンドウを制御するクラスです。
    /// </summary>
    public class MenuEquipmentWindowController : MonoBehaviour, IMenuWindowController
    {
        /// <summary>
        /// メニュー画面に関する機能を管理するクラスへの参照です。
        /// </summary>
        MenuManager _menuManager;

        /// <summary>
        /// メニューの装備画面で装備する場所の選択画面を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentPartsWindowController _partsWindowController;

        /// <summary>
        /// メニューの装備画面で装備するアイテムの選択画面を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentSelectionWindowController _selectionWindowController;

        /// <summary>
        /// メニューの装備画面で情報表示のウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentInformationWindowController _informationWindowController;

        /// <summary>
        /// 装備画面の親オブジェクトへの参照です。
        /// </summary>
        [SerializeField]
        GameObject _equipmentParent;

        /// <summary>
        /// 選択された装備箇所です。
        /// </summary>
        public EquipmentParts SelectedParts { get; private set; }

        /// <summary>
        /// アイテムを装備していない時の表示名です。
        /// </summary>
        public readonly string MissingItemName = "なし";

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        void Update()
        {
            
        }

        /// <summary>
        /// 情報表示ウィンドウへの参照を取得します。
        /// </summary>
        public MenuEquipmentInformationWindowController GetInformationWindowController()
        {
            return _informationWindowController;
        }

        /// <summary>
        /// 指定されたアイテムIDのアイテム名を取得します。
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        public string GetItemName(int itemId)
        {
            string itemName = MissingItemName;
            var item = ItemDataManager.GetItemDataById(itemId);
            if (item != null)
            {
                itemName = item.itemName;
            }
            return itemName;
        }

        /// <summary>
        /// 装備箇所が選択された時のコールバックです。
        /// </summary>
        /// <param name="equipmentParts">装備箇所</param>
        public void OnSelectedEquipmentParts(EquipmentParts equipmentParts)
        {
            SelectedParts = equipmentParts;
            _partsWindowController.SetCanSelectState(false);
            ShowSelectionWindow();
        }

        /// <summary>
        /// 装備するアイテムの選択画面を表示します。
        /// </summary>
        void ShowSelectionWindow()
        {
            StartCoroutine(ShowSelectionWindowProcess());
        }

        /// <summary>
        /// 装備するアイテムの選択画面を表示します。
        /// </summary>
        IEnumerator ShowSelectionWindowProcess()
        {
            yield return null;
            _selectionWindowController.SetUpController(_menuManager);
            _selectionWindowController.SetUpWindow(this);
            _selectionWindowController.SetPageElement();
            _selectionWindowController.SetCanSelectState(true);
            _selectionWindowController.ShowWindow();
        }

        /// <summary>
        /// 装備するアイテムが選択された時のコールバックです。
        /// </summary>
        public void OnSelectedEquipmentItem()
        {
            _selectionWindowController.SetCanSelectState(false);
            _selectionWindowController.HideWindow();

            ShowWindow();
        }

        /// <summary>
        /// 装備するアイテム画面でキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnCanceledSelect()
        {
            _selectionWindowController.SetCanSelectState(false);
            _selectionWindowController.HideWindow();

            ShowWindow();
        }

        /// <summary>
        /// 装備ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _equipmentParent.SetActive(true);

            // 装備画面を表示する際は、装備選択画面で装備を選択できるようにします。
            _partsWindowController.ShowWindow();
            _partsWindowController.SetCanSelectState(true);
            _partsWindowController.SetUpController(_menuManager);
            _partsWindowController.SetUpWindow(this);

            _informationWindowController.ShowWindow();
        }

        /// <summary>
        /// 装備ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _equipmentParent.SetActive(false);

            _partsWindowController.SetCanSelectState(false);
            _partsWindowController.HideWindow();

            _selectionWindowController.SetCanSelectState(false);
            _selectionWindowController.HideWindow();
        }
    }
}