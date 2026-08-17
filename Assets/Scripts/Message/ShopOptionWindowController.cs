namespace SimpleRpg
{
    /// <summary>
    /// お店の選択肢ウィンドウの動作を制御するクラスです。
    /// FindAnyObjectByTypeを使用して取得できるように、OptionWindowControllerを継承した別クラスとして実装しています。
    /// </summary>
    public class ShopOptionWindowController : OptionWindowController
    {
        /// <summary>
        /// 選択結果を通知する先です。
        /// </summary>
        IShopOptionCallback _callback;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="callback">コールバック先</param>
        public void RegisterShopCallback(IShopOptionCallback callback)
        {
            _callback = callback;
        }

        /// <summary>
        /// 決定ボタンが押された時の処理です。
        /// </summary>
        protected override void OnPressedConfirmButton()
        {
            if (_callback != null)
            {
                _callback.OnSelectedShopOption(_selectedIndex);
            }
            StartCoroutine(HideProcess());

            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        protected override void OnPressedCancelButton()
        {
            // キャンセルされた場合は、区別できるように-1を渡します。
            int canceledIndex = -1;
            if (_callback != null)
            {
                _callback.OnSelectedShopOption(canceledIndex);
            }
            StartCoroutine(HideProcess());

            // キャンセル時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Cancel);
        }
    }
}