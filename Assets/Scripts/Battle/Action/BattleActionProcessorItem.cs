using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のアイテムアクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorItem : MonoBehaviour, IBattleActionProcessor
    {
        /// <summary>
        /// 戦闘中のアクションを処理するクラスへの参照です。
        /// </summary>
        BattleActionProcessor _actionProcessor;

        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// メッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MessageWindowController _messageWindowController;

        /// <summary>
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
        /// </summary>
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// 参照をセットします。
        /// </summary>
        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        /// <summary>
        /// アイテムのアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            var itemData = ItemDataManager.GetItemDataById(action.itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {action.itemId}");
                return;
            }

            // 消費アイテムの場合、所持数を減らします。
            if (action.isActorFriend && itemData.itemCategory == ItemCategory.ConsumableItem)
            {
                CharacterStatusManager.UseItem(action.itemId);
            }

            _actionProcessor.SetPauseProcess(true);
            if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.Recovery)
            {
                int hpDelta = BattleCalculator.CalculateHealValue(itemData.itemEffect.value);
                int mpDelta = 0;

                if (action.isActorFriend)
                {
                    CharacterStatusManager.ChangeCharacterStatus(action.targetId, hpDelta, mpDelta);
                }
                else
                {
                    _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, mpDelta);
                }

                StartCoroutine(ShowItemHealMessage(action, itemData.itemName, hpDelta));
            }
            else if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.None)
            {
                StartCoroutine(ShowNoEffectItemMessage(action, itemData.itemName));
            }
            else
            {
                SimpleLogger.Instance.LogWarning($"未定義のアイテム効果です。 ID: {itemData.itemId}");
                StartCoroutine(ShowNoEffectItemMessage(action, itemData.itemName));
            }
        }

        /// <summary>
        /// 回復アイテムのメッセージを表示します。
        /// </summary>
        IEnumerator ShowItemHealMessage(BattleAction action, string itemName, int healValue)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateUseItemMessage(actorName, itemName);
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            // 回復の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Heal);

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateHpHealMessage(targetName, healValue);
            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// アイテムの効果がないときのメッセージを表示します。
        /// </summary>
        IEnumerator ShowNoEffectItemMessage(BattleAction action, string itemName)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateUseItemMessage(actorName, itemName);
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateNoEffectMessage();
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            _actionProcessor.SetPauseProcess(false);
        }
    }
}