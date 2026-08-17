using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleRpg
{
    /// <summary>
    /// お店画面全体を管理するクラスです。
    /// </summary>
    public class ShopManager : MonoBehaviour, IMessageCallback, IOptionCallback, IShopOptionCallback
    {
        /// <summary>
        /// お店の選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopOptionWindowController _shopOptionWindowController;

        /// <summary>
        /// 通常の選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        OptionWindowController _optionWindowController;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MapMessageWindowController _mapMessageWindowController;

        /// <summary>
        /// お店画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopItemWindowController _windowController;

        /// <summary>
        /// お店でアイテムの購入処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopProcessorBuy _shopProcessorBuy;

        /// <summary>
        /// お店でアイテムの売却処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopProcessorSell _shopProcessorSell;

        /// <summary>
        /// お店の処理が完了した際に呼び出されるコールバックです。
        /// </summary>
        IShopCallback _callback;

        /// <summary>
        /// 購入または売却対象のアイテムデータです。
        /// </summary>
        ItemData _selectedItemData;

        /// <summary>
        /// お店のトップ画面を表示するのが2回目以降かどうかのフラグです。
        /// </summary>
        bool _isSecondVisit = false;

        /// <summary>
        /// お店の店主の名前です。
        /// </summary>
        string _shopMasterName = string.Empty;

        /// <summary>
        /// お店に表示するアイテムのIDリストです。
        /// </summary>
        List<int> _shopItems = new();

        /// <summary>
        /// お店に表示するアイテムのIDリストです。
        /// </summary>
        public List<int> ShopItems
        {
            get { return _shopItems; }
        }

        /// <summary>
        /// 選択されたお店のメニューです。
        /// </summary>
        ShopCommand _selectedCommand = ShopCommand.None;

        /// <summary>
        /// 選択されたお店のメニューです。
        /// </summary>
        public ShopCommand SelectedCommand
        {
            get { return _selectedCommand; }
        }

        /// <summary>
        /// お店に入った時のメッセージです。
        /// </summary>
        readonly string WelcomeMessage = "いらっしゃい！\n必要なものはあるかい？";

        /// <summary>
        /// 2回目以降のメッセージです。
        /// </summary>
        readonly string SecondMessage = "他に必要なものはあるかい？";

        /// <summary>
        /// デフォルトの店主の名前です。
        /// </summary>
        readonly string DefaultShopMasterName = "店主";

        /// <summary>
        /// お店の選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        /// <param name="callback">お店の処理が完了した際に呼び出されるコールバック</param>
        /// <param name="shopItemIds">お店に表示するアイテムのIDリスト</param>
        /// <param name="shopMasterName">お店の店主の名前</param>
        public void StartShopProcess(IShopCallback callback, List<int> shopItemIds, string shopMasterName)
        {
            _callback = callback;
            _windowController.SetUpController(this);
            _shopProcessorBuy.SetReferences(_windowController);
            _shopProcessorSell.SetReferences(_windowController);
            _shopItems = shopItemIds;
            _shopMasterName = shopMasterName;

            _isSecondVisit = false;
            _selectedCommand = ShopCommand.None;
            ShowWelcomeMessage();
        }

        /// <summary>
        /// お店に入った時のメッセージを表示します。
        /// </summary>
        public void ShowWelcomeMessage()
        {
            if (_mapMessageWindowController == null)
            {
                SimpleLogger.Instance.LogError("MapMessageWindowControllerがアサインされていません。");
                return;
            }

            string message = _isSecondVisit ? SecondMessage : WelcomeMessage;
            string fullMessage = GetFullMessage(message);
            _mapMessageWindowController.SetUpController(this);
            _mapMessageWindowController.ShowWindow();
            _mapMessageWindowController.ShowGeneralMessage(fullMessage, 0.5f);
        }

        /// <summary>
        /// タグを含めた店主の名前を取得します。
        /// </summary>
        string GetMasterName()
        {
            string masterName = _shopMasterName;
            if (string.IsNullOrEmpty(_shopMasterName))
            {
                // 店主の名前が設定されていない場合はデフォルトの名前を使用します。
                masterName = DefaultShopMasterName;
            }
            return $"<{masterName}>";
        }

        /// <summary>
        /// 店主の名前を含めた表示用のメッセージを取得します。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        string GetFullMessage(string message)
        {
            string masterName = GetMasterName();
            string fullMessage = $"{masterName}\n{message}";
            return fullMessage;
        }

        /// <summary>
        /// メッセージの表示が完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            switch (_selectedCommand)
            {
                case ShopCommand.None:
                    ShowShopOption();
                    break;
            }
        }

        /// <summary>
        /// お店の選択肢ウィンドウを表示します。
        /// </summary>
        public void ShowShopOption()
        {
            if (_shopOptionWindowController == null)
            {
                SimpleLogger.Instance.LogError("ShopOptionWindowControllerがアサインされていません。");
                return;
            }

            _shopOptionWindowController.RegisterShopCallback(this);
            _shopOptionWindowController.SetUpController(this);
            _shopOptionWindowController.ShowWindow();
        }

        /// <summary>
        /// 通常の選択肢ウィンドウを表示します。
        /// </summary>
        public void ShowOption()
        {
            if (_optionWindowController == null)
            {
                SimpleLogger.Instance.LogError("OptionWindowControllerがアサインされていません。");
                return;
            }

            _optionWindowController.SetUpController(this);
            _optionWindowController.ShowWindow();
        }

        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        public void OnSelectedOption(int selectedIndex)
        {
            _mapMessageWindowController.HideWindow();
            if (selectedIndex == 0)
            {
                // はいを選択した場合の処理です。
                if (_selectedCommand == ShopCommand.Buy)
                {
                    _shopProcessorBuy.BuySelectedItem(_selectedItemData);
                }
                else if (_selectedCommand == ShopCommand.Sell)
                {
                    _shopProcessorSell.SellSelectedItem(_selectedItemData);
                }
            }
            else
            {
                // いいえ、またはキャンセルを選択した場合はアイテム選択に戻ります。
                _windowController.SetCanSelectState(true);
            }
        }

        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        public void OnSelectedShopOption(int selectedIndex)
        {
            _mapMessageWindowController.HideWindow();
            if (selectedIndex == 0)
            {
                // 買うを選択した場合の処理です。
                _selectedCommand = ShopCommand.Buy;
                _mapMessageWindowController.HideWindow();
                _windowController.SetUpWindow();
                _windowController.SetPageElement();
                _windowController.ShowWindow();
                StartCoroutine(DelaySetSelectState());
            }
            else if (selectedIndex == 1)
            {
                // 売るを選択した場合の処理です。
                _selectedCommand = ShopCommand.Sell;
                _mapMessageWindowController.HideWindow();
                _windowController.SetUpWindow();
                _windowController.SetPageElement();
                _windowController.ShowWindow();
                StartCoroutine(DelaySetSelectState());
            }
            else
            {
                // キャンセルを選択した場合はイベントに戻ります。
                _selectedCommand = ShopCommand.Exit;
                ShowExitMessage();
            }
        }

        /// <summary>
        /// 選択ウィンドウで選択可能な状態にするための処理です。
        /// </summary>
        IEnumerator DelaySetSelectState()
        {
            yield return null;
            _windowController.SetCanSelectState(true);
        }

        /// <summary>
        /// アイテムが選択された時の処理です。
        /// </summary>
        /// <param name="selectedItemData">選択されたアイテムデータ</param>
        public void OnSelectedItem(ItemData selectedItemData)
        {
            if (selectedItemData == null)
            {
                SimpleLogger.Instance.LogError("購入対象のアイテムがnullです。");
                return;
            }

            _selectedItemData = selectedItemData;

            if (_selectedCommand == ShopCommand.Buy)
            {
                ShowBuyMessage(selectedItemData);
            }
            else if (_selectedCommand == ShopCommand.Sell)
            {
                ShowSellMessage(selectedItemData);
            }
        }

        /// <summary>
        /// 売却時のメッセージを表示します。
        /// </summary>
        /// <param name="selectedItemData">選択されたアイテムデータ</param>
        void ShowSellMessage(ItemData selectedItemData)
        {
            if (selectedItemData == null)
            {
                SimpleLogger.Instance.LogError("売却対象のアイテムがnullです。");
                return;
            }

            _selectedItemData = selectedItemData;
            string itemName = selectedItemData.itemName;
            int price = (int)(selectedItemData.price * ValueSettings.SellPriceMultiplier);
            string message = $"{itemName} なら {price} ゴールドで引き取るよ。\n売ってくれるかい？";
            string fullMessage = GetFullMessage(message);
            _mapMessageWindowController.ShowWindow();
            _mapMessageWindowController.ShowGeneralMessage(fullMessage, 0.5f);
            ShowOption();
        }

        /// <summary>
        /// 購入時のメッセージを表示します。
        /// </summary>
        /// <param name="selectedItemData">選択されたアイテムデータ</param>
        public void ShowBuyMessage(ItemData selectedItemData)
        {
            if (selectedItemData == null)
            {
                SimpleLogger.Instance.LogError("購入対象のアイテムがnullです。");
                return;
            }

            _selectedItemData = selectedItemData;
            string itemName = selectedItemData.itemName;
            string message = $"{itemName} なら {selectedItemData.price} ゴールドだよ。\n買っていくかい？";
            string fullMessage = GetFullMessage(message);
            _mapMessageWindowController.ShowWindow();
            _mapMessageWindowController.ShowGeneralMessage(fullMessage, 0.5f);
            ShowOption();
        }

        /// <summary>
        /// 購入または売却後の処理を行います。
        /// </summary>
        public void PostAction()
        {
            StartCoroutine(PostMessageProcess());
        }

        /// <summary>
        /// 購入または売却後のメッセージ表示を行います。
        /// </summary>
        IEnumerator PostMessageProcess()
        {
            yield return null;
            string message = $"まいどあり！";
            string fullMessage = GetFullMessage(message);
            _mapMessageWindowController.ShowWindow();
            float waitTime = 1.0f;
            _mapMessageWindowController.ShowGeneralMessage(fullMessage, waitTime);
            yield return new WaitForSeconds(waitTime);

            _mapMessageWindowController.HideWindow();
            _isSecondVisit = true;
            _selectedCommand = ShopCommand.None;
            ShowWelcomeMessage();
        }

        /// <summary>
        /// 退店時のメッセージを表示します。
        /// </summary>
        void ShowExitMessage()
        {
            StartCoroutine(WaitExitMessage());
        }

        /// <summary>
        /// 退店時のメッセージを表示します。
        /// </summary>
        IEnumerator WaitExitMessage()
        {
            yield return null;
            string message = $"またおいで！";
            string fullMessage = GetFullMessage(message);
            _mapMessageWindowController.ShowWindow();
            _mapMessageWindowController.ShowPager();
            _mapMessageWindowController.ShowGeneralMessage(fullMessage, 0.5f);
            _mapMessageWindowController.StartKeyWait();
            while (_mapMessageWindowController.IsWaitingKeyInput)
            {
                yield return null;
            }

            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);

            _mapMessageWindowController.HidePager();
            _mapMessageWindowController.HideWindow();
            ExitProcess();
        }

        /// <summary>
        /// 退店時の処理です。
        /// </summary>
        void ExitProcess()
        {
            // お店の終了を通知します。
            if (_callback != null)
            {
                _callback.OnFinishedShopProcess();
            }
        }

        /// <summary>
        /// 購入画面または売却画面がキャンセルされた時の処理です。
        /// </summary>
        public void OnCanceled()
        {
            _selectedCommand = ShopCommand.None;
            _isSecondVisit = true;
            ShowWelcomeMessage();
        }
    }
}