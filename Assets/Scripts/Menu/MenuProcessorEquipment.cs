using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// メニューウィンドウで装備の処理を制御するクラスです。
    /// </summary>
    public class MenuProcessorEquipment : MonoBehaviour
    {
        /// <summary>
        /// メニュー画面の装備ウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentWindowController _windowController;

        /// <summary>
        /// メニューの装備画面で装備するアイテムの選択画面を制御するクラスへの参照です。
        /// </summary>
        MenuEquipmentSelectionWindowController _selectionWindowController;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(MenuEquipmentWindowController windowController, MenuEquipmentSelectionWindowController selectionWindowController)
        {
            _windowController = windowController;
            _selectionWindowController = selectionWindowController;
        }

        /// <summary>
        /// 引数のIDのアイテムを装備します。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        public void EquipmentSelectedItem(int itemId)
        {
            // 装備を外す項目を選択している場合は、アイテム定義の有無を確認しません。
            if (itemId != CharacterStatusManager.NoEquipmentId)
            {
                var itemData = ItemDataManager.GetItemDataById(itemId);
                if (itemData == null)
                {
                    SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {itemId}");
                    return;
                }
            }

            int characterId = CharacterStatusManager.partyCharacter[0];
            var status = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (status == null)
            {
                SimpleLogger.Instance.LogWarning($"キャラクターデータが見つかりませんでした。 ID: {characterId}");
                return;
            }

            int previousEquipId = 0;
            if (_windowController.SelectedParts == EquipmentParts.Weapon)
            {
                // 以前装備していた武器をパーティの所持アイテムに戻します。
                previousEquipId = status.equipWeaponId;
                status.equipWeaponId = itemId;
            }
            else if (_windowController.SelectedParts == EquipmentParts.Armor)
            {
                // 以前装備していた防具をパーティの所持アイテムに戻します。
                previousEquipId = status.equipArmorId;
                status.equipArmorId = itemId;
            }
            CharacterStatusManager.IncreaseItem(previousEquipId, 1);
            CharacterStatusManager.DecreaseItem(itemId, 1);

            _selectionWindowController.PostAction();
        }
    }
}