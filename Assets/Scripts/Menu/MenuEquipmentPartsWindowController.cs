using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニューの装備画面で装備する場所の選択画面を制御するクラスです。
    /// </summary>
    public class MenuEquipmentPartsWindowController : MonoBehaviour, IMenuWindowController
    {
        /// <summary>
        /// メニューの装備画面で装備する場所の選択画面のUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentPartsUIController _uiController;

        /// <summary>
        /// メニュー画面全体を管理するクラスへの参照です。
        /// </summary>
        MenuManager _menuManager;

        /// <summary>
        /// メニュー画面の装備ウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentWindowController _windowController;

        /// <summary>
        /// メニューの装備画面で情報表示のウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentInformationWindowController _informationWindowController;

        /// <summary>
        /// 現在選択中の装備箇所です。
        /// </summary>
        EquipmentParts _selectedParts;

        /// <summary>
        /// 装備可能な項目の数です。
        /// </summary>
        int _partsCount;

        /// <summary>
        /// 装備箇所の選択が可能かどうかを示すフラグです。
        /// </summary>
        bool _canSelectParts;

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

            SetItemName();
            SetStatus();
            InitializeSelect();
        }

        void Update()
        {
            SelectParts();
        }

        /// <summary>
        /// 装備箇所を選択します。
        /// </summary>
        void SelectParts()
        {
            if (_menuManager == null || _windowController == null)
            {
                return;
            }

            if (_menuManager.SelectedMenu != MenuCommand.Equipment)
            {
                return;
            }

            if (!_canSelectParts)
            {
                return;
            }

            if (InputGameKey.MenuUp())
            {
                SetPreParts();
            }
            else if (InputGameKey.MenuDown())
            {
                SetNextParts();
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
        /// ひとつ前の装備箇所を選択します。
        /// </summary>
        void SetPreParts()
        {
            int currentParts = (int)_selectedParts;
            int nextParts = currentParts - 1;
            if (nextParts < 0)
            {
                nextParts = _partsCount - 1;
            }
            _selectedParts = (EquipmentParts)nextParts;

            PostSelect();
        }

        /// <summary>
        /// ひとつ後の装備箇所を選択します。
        /// </summary>
        void SetNextParts()
        {
            int currentParts = (int)_selectedParts;
            int nextParts = currentParts + 1;
            if (nextParts > _partsCount - 1)
            {
                nextParts = 0;
            }
            _selectedParts = (EquipmentParts)nextParts;

            PostSelect();
        }

        /// <summary>
        /// 項目選択後の処理です。
        /// </summary>
        void PostSelect()
        {
            SetDescription();
            _uiController.ShowSelectedCursor(_selectedParts);
        }

        /// <summary>
        /// 選択されたアイテムの説明を表示します。
        /// </summary>
        void SetDescription()
        {
            int characterId = CharacterStatusManager.partyCharacter[0];
            var status = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (status == null)
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
                return;
            }

            int itemId = 0;
            if (_selectedParts == EquipmentParts.Weapon)
            {
                itemId = status.equipWeaponId;
            }
            else if (_selectedParts == EquipmentParts.Armor)
            {
                itemId = status.equipArmorId;
            }

            _informationWindowController.SetDescription(itemId);
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);

            _windowController.OnSelectedEquipmentParts(_selectedParts);
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

            _menuManager.OnEquipmentCanceled();
            HideWindow();
            _windowController.HideWindow();
        }

        /// <summary>
        /// 現在装備中のアイテム名を表示します。
        /// </summary>
        public void SetItemName()
        {
            int characterId = CharacterStatusManager.partyCharacter[0];
            var status = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (status == null)
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
                return;
            }

            string weaponName = _windowController.GetItemName(status.equipWeaponId);
            string armorName = _windowController.GetItemName(status.equipArmorId);

            _uiController.SetWeaponName(weaponName);
            _uiController.SetArmorName(armorName);
        }

        /// <summary>
        /// 装備品を含めたキャラクターのステータスを表示します。
        /// </summary>
        public void SetStatus()
        {
            int characterId = CharacterStatusManager.partyCharacter[0];
            _informationWindowController.SetCharacterName(characterId);
            _informationWindowController.SetStatusValue(characterId);
        }

        /// <summary>
        /// 装備箇所選択を初期化します。
        /// </summary>
        public void InitializeSelect()
        {
            _selectedParts = EquipmentParts.Weapon;
            
            _partsCount = System.Enum.GetNames(typeof(EquipmentParts)).Length;
            PostSelect();
        }

        /// <summary>
        /// 装備箇所の選択が可能かどうかのフラグをセットします。
        /// </summary>
        /// <param name="canSelect">装備箇所の選択が可能かどうか</param>
        public void SetCanSelectState(bool canSelect)
        {
            _canSelectParts = canSelect;
        }

        /// <summary>
        /// 装備箇所ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// 装備箇所ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}
