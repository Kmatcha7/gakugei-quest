using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニューのトップ画面を制御するクラスです。
    /// </summary>
    public class TopMenuWindowController : MonoBehaviour, IMenuWindowController
    {
        /// <summary>
        /// メニューのトップ画面のUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TopMenuUIController _uiController;

        /// <summary>
        /// メニュー画面に関する機能を管理するクラスへの参照です。
        /// </summary>
        MenuManager _menuManager;

        /// <summary>
        /// 現在選択中のメニューです。
        /// </summary>
        MenuCommand _selectedCommand;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        void Update()
        {
            SelectCommand();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectCommand()
        {
            if (_menuManager == null)
            {
                return;
            }

            if (_menuManager.MenuPhase != MenuPhase.Top)
            {
                return;
            }

            if (InputGameKey.MenuUp())
            {
                SetPreCommand();
                _uiController.ShowSelectedCursor(_selectedCommand);
            }
            else if (InputGameKey.MenuDown())
            {
                SetNextCommand();
                _uiController.ShowSelectedCursor(_selectedCommand);
            }
            else if (InputGameKey.ConfirmButton())
            {
                // 選択時の効果音を再生します。
                AudioManager.Instance.PlaySe(SeNames.OK);
                _menuManager.OnSelectedMenu(_selectedCommand);
            }
            else if (InputGameKey.CancelButton())
            {
                // キャンセル時の効果音を再生します。
                AudioManager.Instance.PlaySe(SeNames.Cancel);
                CloseMenu();
            }
        }

        /// <summary>
        /// ひとつ前のコマンドを選択します。
        /// </summary>
        void SetPreCommand()
        {
            int currentCommand = (int)_selectedCommand;
            int nextCommand = currentCommand - 1;
            if (nextCommand < 0)
            {
                nextCommand = 6;
            }
            _selectedCommand = (MenuCommand)nextCommand;
        }

        /// <summary>
        /// ひとつ後のコマンドを選択します。
        /// </summary>
        void SetNextCommand()
        {
            int currentCommand = (int)_selectedCommand;
            int nextCommand = currentCommand + 1;
            if (nextCommand > 6)
            {
                nextCommand = 0;
            }
            _selectedCommand = (MenuCommand)nextCommand;
        }

        /// <summary>
        /// 選択を初期化します。
        /// </summary>
        public void InitializeCommand()
        {
            _selectedCommand = MenuCommand.Item;
            _uiController.ShowSelectedCursor(_selectedCommand);
        }

        /// <summary>
        /// メニュー画面を閉じます。
        /// </summary>
        public void CloseMenu()
        {
            StartCoroutine(CloseMenuProcess());
        }

        /// <summary>
        /// メニュー画面を閉じます。
        /// </summary>
        IEnumerator CloseMenuProcess()
        {
            yield return null;
            _menuManager.OnCloseMenu();
            HideWindow();
        }

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}
