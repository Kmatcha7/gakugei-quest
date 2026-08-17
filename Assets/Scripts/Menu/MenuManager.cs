using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面全体を管理するクラスです。
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// メニューのトップ画面を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TopMenuWindowController _topMenuWindowController;

        /// <summary>
        /// メニュー画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuItemWindowController _menuItemWindowController;

        /// <summary>
        /// メニュー画面の装備ウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentWindowController _menuEquipmentWindowController;

        /// <summary>
        /// メニュー画面のステータスウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuStatusWindowController _menuStatusWindowController;

        /// <summary>
        /// メニューのセーブ画面のウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuSaveWindowController _menuSaveWindowController;

        /// <summary>
        /// メニューのゲーム終了画面のウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuQuitGameWindowController _menuQuitGameWindowController;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MapMessageWindowController _mapMessageWindowController;

        /// <summary>
        /// メニューのフェーズです。
        /// </summary>
        public MenuPhase MenuPhase { get; private set; }

        /// <summary>
        /// 選択されたメニューです。
        /// </summary>
        public MenuCommand SelectedMenu { get; private set; }

        void Start()
        {
            
        }

        /// <summary>
        /// メッセージウィンドウへの参照を返します。
        /// </summary>
        public MapMessageWindowController GetMessageWindowController()
        {
            return _mapMessageWindowController;
        }

        void Update()
        {
            CheckOpenMenuKey();
        }

        /// <summary>
        /// メニューを開くキーの入力を確認します。
        /// </summary>
        void CheckOpenMenuKey()
        {
            // 移動中以外の場合はメニューを開けないようにします。
            if (GameStateManager.CurrentState != GameState.Moving)
            {
                return;
            }

            // メニューが閉じている場合のみ、メニューを開くキーの入力を確認します。
            if (MenuPhase != MenuPhase.Closed)
            {
                return;
            }
            
            // メニューを開くキーが押された場合、メニューを開きます。
            if (InputGameKey.CancelButton())
            {
                // 選択時の効果音を再生します。
                AudioManager.Instance.PlaySe(SeNames.OK);
                OpenMenu();
            }
        }

        /// <summary>
        /// メニュー画面を表示します。
        /// </summary>
        void OpenMenu()
        {
            StartCoroutine(OpenMenuProcess());
        }

        /// <summary>
        /// メニュー画面を表示します。
        /// </summary>
        IEnumerator OpenMenuProcess()
        {
            yield return null;

            MenuPhase = MenuPhase.Top;
            _topMenuWindowController.SetUpController(this);
            _topMenuWindowController.InitializeCommand();
            _topMenuWindowController.ShowWindow();

            _characterMoverManager.StopCharacterMover();
        }

        /// <summary>
        /// メニュー項目が選択された時のコールバックです。
        /// </summary>
        public void OnSelectedMenu(MenuCommand menuCommand)
        {
            SelectedMenu = menuCommand;
            SimpleLogger.Instance.Log($"選択されたメニュー: {menuCommand}");
            HandleMenu();
        }

        /// <summary>
        /// 選択されたメニューに応じた処理を呼び出します。
        /// </summary>
        void HandleMenu()
        {
            switch (SelectedMenu)
            {
                case MenuCommand.Item:
                case MenuCommand.Magic:
                    ShowItemMenu();
                    break;
                case MenuCommand.Equipment:
                    ShowEquipmentMenu();
                    break;
                case MenuCommand.Status:
                    ShowStatusMenu();
                    break;
                case MenuCommand.Save:
                    ShowSaveMenu();
                    break;
                case MenuCommand.QuitGame:
                    ShowQuitMenu();
                    break;
                case MenuCommand.Close:
                    _topMenuWindowController.CloseMenu();
                    break;
            }
        }

        /// <summary>
        /// アイテムメニューを表示します。
        /// </summary>
        void ShowItemMenu()
        {
            MenuPhase = MenuPhase.Item;
            _menuItemWindowController.SetUpController(this);
            _menuItemWindowController.SetUpWindow();
            _menuItemWindowController.SetPageElement();
            _menuItemWindowController.ShowWindow();
            _menuItemWindowController.SetCanSelectState(true);
        }

        /// <summary>
        /// 装備メニューを表示します。
        /// </summary>
        void ShowEquipmentMenu()
        {
            MenuPhase = MenuPhase.Equipment;
            _menuEquipmentWindowController.SetUpController(this);
            _menuEquipmentWindowController.ShowWindow();
        }

        /// <summary>
        /// ステータスメニューを表示します。
        /// </summary>
        void ShowStatusMenu()
        {
            MenuPhase = MenuPhase.Status;
            _menuStatusWindowController.SetUpController(this);
            _menuStatusWindowController.ShowWindow();
        }

        /// <summary>
        /// セーブメニューを表示します。
        /// </summary>
        void ShowSaveMenu()
        {
            MenuPhase = MenuPhase.Save;
            _menuSaveWindowController.SetUpController(this);
            _menuSaveWindowController.ShowWindow();
        }

        /// <summary>
        /// ゲーム終了メニューを表示します。
        /// </summary>
        void ShowQuitMenu()
        {
            MenuPhase = MenuPhase.QuitGame;
            _menuQuitGameWindowController.SetUpController(this);
            _menuQuitGameWindowController.ShowWindow();
        }

        /// <summary>
        /// メニュー画面が閉じる時のコールバックです。
        /// </summary>
        public void OnCloseMenu()
        {
            MenuPhase = MenuPhase.Closed;
            _characterMoverManager.ResumeCharacterMover();
        }

        /// <summary>
        /// 項目選択画面でキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnItemCanceled()
        {
            MenuPhase = MenuPhase.Top;
        }

        /// <summary>
        /// 装備選択画面でキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnEquipmentCanceled()
        {
            MenuPhase = MenuPhase.Top;
        }

        /// <summary>
        /// ステータス画面でキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnStatusCanceled()
        {
            MenuPhase = MenuPhase.Top;
        }

        /// <summary>
        /// セーブ画面でキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnSaveCanceled()
        {
            MenuPhase = MenuPhase.Top;
        }

        /// <summary>
        /// ゲーム終了画面でキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnQuitCanceled()
        {
            MenuPhase = MenuPhase.Top;
        }
    }
}