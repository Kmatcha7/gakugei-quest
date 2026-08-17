using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 選択肢に関するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessShowOption : EventProcessBase, IOptionCallback
    {
        /// <summary>
        /// 選択肢がはいの場合の次の処理です。
        /// </summary>
        [SerializeField]
        EventProcessBase _nextProcessYes;

        /// <summary>
        /// 選択肢がいいえの場合の次の処理です。
        /// </summary>
        [SerializeField]
        EventProcessBase _nextProcessNo;

        /// <summary>
        /// デフォルトで選択されている選択肢のインデックスです。
        /// </summary>
        [SerializeField]
        int _optionIndexNo;

        /// <summary>
        /// このプロセスの終了後にメッセージウィンドウを開いたままにするかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _hideMessageWindow = true;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MapMessageWindowController _messageWindowController;

        /// <summary>
        /// 選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        OptionWindowController _optionWindowController;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            SetUpReference();
            if (_optionWindowController == null)
            {
                SimpleLogger.Instance.LogError("OptionWindowControllerが見つかりません。");
                CallNextProcess();
                return;
            }

            ShowOption();
        }

        /// <summary>
        /// メッセージのコントローラへの参照をセットします。
        /// </summary>
        void SetUpReference()
        {
            if (_messageWindowController == null)
            {
                _messageWindowController = FindAnyObjectByType<MapMessageWindowController>();
            }

            if (_optionWindowController == null)
            {
                var optionWindows = FindObjectsByType<OptionWindowController>(FindObjectsSortMode.None);
                foreach (var optionWindow in optionWindows)
                {
                    // 取得したOptionWindowControllerのうち、派生クラスでないものを使用します。
                    if (optionWindow.GetType() == typeof(OptionWindowController))
                    {
                        _optionWindowController = optionWindow;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 選択肢ウィンドウを表示します。
        /// </summary>
        void ShowOption()
        {
            _optionWindowController.SetUpController(this, _optionIndexNo);
            _optionWindowController.ShowWindow();
        }

        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        public void OnSelectedOption(int selectedIndex)
        {
            // 選択肢に応じた次の処理を選択します。
            // キャンセルの場合はデフォルトの次の処理を使用します。
            EventProcessBase nextProcess = _nextProcess;
            if (selectedIndex == 0)
            {
                // はいが選ばれた場合
                nextProcess = _nextProcessYes;
            }
            else if (selectedIndex == 1)
            {
                // いいえが選ばれた場合
                nextProcess = _nextProcessNo;
            }
            HideMessageWindow();
            CallNextProcess(nextProcess);
        }

        /// <summary>
        /// メッセージウィンドウを非表示にします。
        /// </summary>
        void HideMessageWindow()
        {
            if (!_hideMessageWindow)
            {
                return;
            }

            if (_messageWindowController == null)
            {
                SimpleLogger.Instance.LogError("MapMessageWindowControllerが見つかりません。");
                return;
            }

            _messageWindowController.HideWindow();
            _messageWindowController.HidePager();
        }
    }
}