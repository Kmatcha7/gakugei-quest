using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// お店でのアイテム売買に関するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessShop : EventProcessBase, IShopCallback
    {
        /// <summary>
        /// お店の店主の名前です。
        /// </summary>
        [SerializeField]
        string _shopMasterName;

        /// <summary>
        /// お店で買えるアイテムのIDリストです。
        /// </summary>
        [SerializeField]
        List<int> _shopItemIds = new List<int>();

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            var shopManager = FindAnyObjectByType<ShopManager>();
            if (shopManager == null)
            {
                SimpleLogger.Instance.LogError("ShopManagerが見つかりません。");
                CallNextProcess();
                return;
            }

            shopManager.StartShopProcess(this, _shopItemIds, _shopMasterName);
        }

        /// <summary>
        /// お店の処理が終了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedShopProcess()
        {
            CallNextProcess();
        }
    }
}