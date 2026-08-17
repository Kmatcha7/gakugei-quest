using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// お店のアイテムUIを制御するクラスです。
    /// </summary>
    public class ShopItemUIController : MenuItemUIController
    {
        /// <summary>
        /// お店のアイテムUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _goldText;

        /// <summary>
        /// お店のアイテムUIを制御するクラスです。
        /// </summary>
        public void SetGoldText(int gold)
        {
            _goldText.text = gold.ToString();
        }
    }
}