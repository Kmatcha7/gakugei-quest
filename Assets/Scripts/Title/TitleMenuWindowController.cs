using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面のメニューのウィンドウを制御するクラスです。
    /// </summary>
    public class TitleMenuWindowController : MonoBehaviour
    {
        /// <summary>
        /// タイトル画面のメニューのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TitleMenuUIController _uiController;

        /// <summary>
        /// タイトル画面のメニューを管理するクラスへの参照です。
        /// </summary>
        TitleMenuManager _titleMenuManager;

        /// <summary>
        /// 現在選択中のメニューです。
        /// </summary>
        TitleCommand _selectedCommand;

        /// <summary>
        /// メニューを選択できるかどうかのフラグです。
        /// </summary>
        bool _canSelect;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(TitleMenuManager titleMenuManager)
        {
            _titleMenuManager = titleMenuManager;
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
            if (_titleMenuManager == null)
            {
                return;
            }

            if (!_canSelect)
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
                OnPressedConfirmButton();
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
                int commandCount = System.Enum.GetValues(typeof(TitleCommand)).Length;
                nextCommand = commandCount - 1;
            }
            _selectedCommand = (TitleCommand)nextCommand;
        }

        /// <summary>
        /// ひとつ後のコマンドを選択します。
        /// </summary>
        void SetNextCommand()
        {
            int currentCommand = (int)_selectedCommand;
            int nextCommand = currentCommand + 1;
            int commandCount = System.Enum.GetValues(typeof(TitleCommand)).Length;
            if (nextCommand >= commandCount)
            {
                nextCommand = 0;
            }
            _selectedCommand = (TitleCommand)nextCommand;
        }

        /// <summary>
        /// 選択を初期化します。
        /// </summary>
        public void InitializeCommand()
        {
            _selectedCommand = TitleCommand.Start;
            _uiController.ShowSelectedCursor(_selectedCommand);
        }

        /// <summary>
        /// メニューを選択できるかどうかの状態を設定します。
        /// </summary>
        public void SetCanSelectState(bool canSelect)
        {
            _canSelect = canSelect;
        }

        /// <summary>
        /// 決定ボタンが押された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);

            SetCanSelectState(false);
            _titleMenuManager.OnSelectedMenu(_selectedCommand);
        }
    }
}
