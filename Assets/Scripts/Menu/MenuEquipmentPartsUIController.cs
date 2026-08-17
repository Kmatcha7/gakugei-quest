using System.Collections;
using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// メニューの装備画面で装備する場所の選択画面のUIを制御するクラスです。
    /// </summary>
    public class MenuEquipmentPartsUIController : MonoBehaviour, IMenuUIController
    {
        /// <summary>
        /// 武器スロットのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjWeapon;

        /// <summary>
        /// 防具スロットのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjArmor;

        /// <summary>
        /// 武器スロットのアイテム名テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _weaponNameText;

        /// <summary>
        /// 防具スロットのアイテム名テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _armorNameText;

        /// <summary>
        /// 装備箇所のカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _cursorObjWeapon.SetActive(false);
            _cursorObjArmor.SetActive(false);
        }

        /// <summary>
        /// 選択中の装備箇所のカーソルを表示します。
        /// </summary>
        /// <param name="equipmentParts">装備箇所</param>
        public void ShowSelectedCursor(EquipmentParts equipmentParts)
        {
            HideAllCursor();

            switch (equipmentParts)
            {
                case EquipmentParts.Weapon:
                    _cursorObjWeapon.SetActive(true);
                    break;
                case EquipmentParts.Armor:
                    _cursorObjArmor.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// 武器の名前をセットします。
        /// </summary>
        /// <param name="name">武器の名前</param>
        public void SetWeaponName(string name)
        {
            _weaponNameText.text = name;
        }

        /// <summary>
        /// 防具の名前をセットします。
        /// </summary>
        /// <param name="name">防具の名前</param>
        public void SetArmorName(string name)
        {
            _armorNameText.text = name;
        }

        /// <summary>
        /// UIを表示します。
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}