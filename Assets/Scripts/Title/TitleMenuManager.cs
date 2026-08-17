using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面のメニューを管理するクラスです。
    /// </summary>
    public class TitleMenuManager : MonoBehaviour
    {
        /// <summary>
        /// タイトルメニューのトップ画面を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TitleMenuWindowController _titleMenuWindowController;

        /// <summary>
        /// タイトル画面のはじめからのメニューを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TitleStartController _titleStartController;

        /// <summary>
        /// タイトル画面のつづきからのメニューを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TitleContinueController _titleContinueController;

        /// <summary>
        /// タイトル画面のゲームの終了メニューを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TitleQuitGameController _titleQuitGameController;

        /// <summary>
        /// 選択されたメニューです。
        /// </summary>
        public TitleCommand SelectedMenu { get; private set; }

        void Start()
        {
            InitializeMenu();
        }

        /// <summary>
        /// メニューを初期化します。
        /// </summary>
        public void InitializeMenu()
        {
            _titleMenuWindowController.InitializeCommand();
        }

        /// <summary>
        /// メニューを選択できるようにします。
        /// </summary>
        public void StartSelect()
        {
            _titleMenuWindowController.SetUpController(this);
            _titleMenuWindowController.SetCanSelectState(true);
        }

        /// <summary>
        /// メニュー項目が選択された時のコールバックです。
        /// </summary>
        public void OnSelectedMenu(TitleCommand titleCommand)
        {
            SelectedMenu = titleCommand;
            SimpleLogger.Instance.Log($"選択されたメニュー: {titleCommand}");
            HandleMenu();
        }

        /// <summary>
        /// 選択されたメニューに応じた処理を呼び出します。
        /// </summary>
        void HandleMenu()
        {
            switch (SelectedMenu)
            {
                case TitleCommand.Start:
                    _titleStartController.StartNewGame();
                    break;
                case TitleCommand.Continue:
                    _titleContinueController.SetUpController(this);
                    _titleContinueController.ShowWindow();
                    break;
                case TitleCommand.Quit:
                    _titleQuitGameController.QuitGame();
                    break;
            }
        }

        /// <summary>
        /// ロード画面でキャンセルされた時のコールバックです。
        /// </summary>
        public void OnLoadCanceled()
        {
            InitializeMenu();
            StartSelect();
        }

        /// <summary>
        /// ロード画面でセーブ枠が選択された時のコールバックです。
        /// </summary>
        public void OnSelectedSlotId(int slotId)
        {
            _titleStartController.ContinueGame(slotId);
        }
    }
}