using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// お店でアイテムの売却処理を制御するクラスです。
    /// </summary>
    public class ShopProcessorSell : MonoBehaviour
    {
        /// <summary>
        /// メニューウィンドウにて売却に関する処理を制御するクラスへの参照です。
        /// </summary>
        ShopItemWindowSellController _sellController;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(ShopItemWindowController windowController)
        {
            _sellController = windowController.GetSellController();
        }

        /// <summary>
        /// 引数のアイテムを売却します。
        /// </summary>
        /// <param name="itemData">売却するアイテムのデータ</param>
        public void SellSelectedItem(ItemData itemData)
        {
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。");
                return;
            }

            // アイテムの所持数を減らします。
            int itemNum = 1;
            CharacterStatusManager.DecreaseItem(itemData.itemId, itemNum);

            // アイテムの売却に伴うお金の増加を処理します。
            int price = (int) (itemData.price * ValueSettings.SellPriceMultiplier);
            CharacterStatusManager.IncreaseGold(price * itemNum);

            _sellController.OnFinishedItemProcess();
        }
    }
}