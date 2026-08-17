using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// メニューウィンドウでアイテムの使用処理を制御するクラスです。
    /// </summary>
    public class MenuProcessorItem : MonoBehaviour
    {
        /// <summary>
        /// メニュー画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuItemWindowController _windowController;

        /// <summary>
        /// メニューウィンドウにてアイテムに関する処理を制御するクラスへの参照です。
        /// </summary>
        MenuItemWindowItemController _itemController;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MapMessageWindowController _mapMessageWindowController;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(MenuItemWindowController windowController)
        {
            _windowController = windowController;
            _itemController = windowController.GetItemController();
            _mapMessageWindowController = windowController.GetMenuManager().GetMessageWindowController();
        }

        /// <summary>
        /// 引数のIDのアイテムを使用します。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        public void UseSelectedItem(int itemId)
        {
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {itemId}");
                return;
            }

            // 消費アイテムの場合、所持数を減らします。
            if (itemData.itemCategory == ItemCategory.ConsumableItem)
            {
                CharacterStatusManager.UseItem(itemData.itemId);
            }

            int targetCharacterId = CharacterStatusManager.partyCharacter[0];
            if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.Recovery)
            {
                int hpDelta = BattleCalculator.CalculateHealValue(itemData.itemEffect.value);
                int mpDelta = 0;
                CharacterStatusManager.ChangeCharacterStatus(targetCharacterId, hpDelta, mpDelta);
                StartCoroutine(ShowItemHealMessage(targetCharacterId, itemData.itemName, hpDelta));
            }
        }

        /// <summary>
        /// 回復アイテムのメッセージを表示します。
        /// </summary>
        IEnumerator ShowItemHealMessage(int characterId, string itemName, int healValue)
        {
            var characterName = CharacterDataManager.GetCharacterName(characterId);
            string actorName = characterName;
            string targetName = characterName;

            _mapMessageWindowController.SetUpController(_windowController);
            _mapMessageWindowController.HidePager();
            _mapMessageWindowController.ShowWindow();

            _windowController.SetCanSelectState(false);
            _windowController.SetPauseMessageState(true);
            _mapMessageWindowController.GenerateUseItemMessage(actorName, itemName);
            while (_windowController.IsPausedMessage)
            {
                yield return null;
            }

            // 回復の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Heal);

            _windowController.SetPauseMessageState(true);
            _mapMessageWindowController.GenerateHpHealMessage(targetName, healValue);
            _windowController.UpdateStatus();
            while (_windowController.IsPausedMessage)
            {
                yield return null;
            }

            // キー入力を待ちます。
            _mapMessageWindowController.StartKeyWait();
            while (_mapMessageWindowController.IsWaitingKeyInput)
            {
                yield return null;
            }

            _mapMessageWindowController.HideWindow();
            _itemController.OnFinishedItemProcess();
        }
    }
}