using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の魔法アクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorMagic : MonoBehaviour, IBattleActionProcessor
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
        BattleSpriteController _battleSpriteController;
        /// <summary>
        /// 魔法効果をポーズするかどうかのフラグです。
        /// </summary>
        bool _pauseMagicEffect;

        /// <summary>
        /// 参照をセットします。
        /// </summary>
        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
            _battleSpriteController = _battleManager.GetBattleSpriteController();
        }

        /// <summary>
        /// 魔法のアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            var magicData = MagicDataManager.GetMagicDataById(action.itemId);

            // 消費MPの分だけMPを減らします。
            int hpDelta = 0;
            int mpDelta = magicData.cost * -1;
            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);
            }

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ProcessMagicActionCoroutine(action, magicData));
        }

        /// <summary>
        /// 魔法のアクションを処理するコルーチンです。
        /// </summary>
        IEnumerator ProcessMagicActionCoroutine(BattleAction action, MagicData magicData)
        {
            // 魔法の詠唱時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Magic);

            // 魔法の効果を処理します。
            foreach (var magicEffect in magicData.magicEffects)
            {
                // メッセージ表示用の行動を生成します。
                BattleAction messageAction = new()
                {
                    actorId = action.actorId,
                    targetId = action.targetId,
                    isActorFriend = action.isActorFriend,
                    isTargetFriend = action.isTargetFriend
                };

                if (magicEffect.magicCategory == MagicCategory.Recovery)
                {
                    int hpDelta = BattleCalculator.CalculateHealValue(magicEffect.value);
                    int mpDelta = 0;
                    bool isMagicTargetFriend = IsMagicTargetFriend(magicEffect);
                    if (isMagicTargetFriend)
                    {
                        messageAction.targetId = action.actorId;
                        messageAction.isTargetFriend = action.isActorFriend;
                    }
                    else
                    {
                        messageAction.targetId = action.targetId;
                        messageAction.isTargetFriend = !action.isActorFriend;
                    }

                    if (messageAction.isTargetFriend)
                    {
                        CharacterStatusManager.ChangeCharacterStatus(messageAction.targetId, hpDelta, mpDelta);
                    }
                    else
                    {
                        _enemyStatusManager.ChangeEnemyStatus(messageAction.targetId, hpDelta, mpDelta);
                    }

                    _pauseMagicEffect = true;
                    StartCoroutine(ShowMagicHealMessage(messageAction, magicData.magicName, hpDelta));
                }
                else if (magicEffect.magicCategory == MagicCategory.Damage)
                {
                    int damageValue =
                        BattleCalculator.CalculateMagicDamage(magicEffect.value);

                    int hpDelta = -damageValue;
                    int mpDelta = 0;

                    messageAction.targetId = action.targetId;
                    messageAction.isTargetFriend = action.isTargetFriend;

                    bool isTargetDefeated = false;

                    // ▼ HP変更＋撃破判定
                    if (messageAction.isTargetFriend)
                    {
                        CharacterStatusManager.ChangeCharacterStatus(
                            messageAction.targetId,
                            hpDelta,
                            mpDelta
                        );

                        isTargetDefeated =
                            CharacterStatusManager.IsCharacterDefeated(
                                messageAction.targetId
                            );
                    }
                    else
                    {
                        _enemyStatusManager.ChangeEnemyStatus(messageAction.targetId, hpDelta, mpDelta);
                        var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(messageAction.targetId);
                        
                        if (enemyStatus != null)
                        {
                            isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(messageAction.targetId);

                            if (isTargetDefeated)
                            {
                                _enemyStatusManager.OnDefeatEnemy(messageAction.targetId);
                            }
                        }
                    }

                    _pauseMagicEffect = true;
                    StartCoroutine(ShowMagicDamageMessage(messageAction,magicData.magicName,damageValue,isTargetDefeated));
                }                
                else if (magicEffect.magicCategory == MagicCategory.None)
                {
                    // 効果がない場合のメッセージを表示します。
                    _pauseMagicEffect = true;
                    StartCoroutine(ShowNoEffectMessage(messageAction, magicData.magicName));
                }
                else
                {
                    SimpleLogger.Instance.LogWarning($"未定義の魔法効果です。 ID: {magicData.magicId}");

                    // 効果がない場合のメッセージを表示します。
                    _pauseMagicEffect = true;
                    StartCoroutine(ShowNoEffectMessage(messageAction, magicData.magicName));
                }

                while (_pauseMagicEffect)
                {
                    yield return null;
                }
            }

            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// 回復魔法のメッセージを表示します。
        /// </summary>
        IEnumerator ShowMagicHealMessage(BattleAction action, string magicName, int healValue)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateMagicCastMessage(actorName, magicName);
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

            _pauseMagicEffect = false;
        }
        IEnumerator ShowMagicDamageMessage(BattleAction action,string magicName,int damageValue,bool isTargetDefeated)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId,action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId,action.isTargetFriend);

            // 詠唱メッセージ
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateMagicCastMessage(actorName,magicName);

            while (_actionProcessor.IsPausedMessage)
                yield return null;
            // ダメージSE
            AudioManager.Instance.PlaySe(SeNames.Damage);

            // ダメージ表示
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateDamageMessage(targetName,damageValue);

            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage)
            yield return null;
            if (!isTargetDefeated)
            {
                _pauseMagicEffect = false;
                yield break;
            }

            if (action.isTargetFriend)
            {
                _actionProcessor.SetPauseMessage(true);
                _messageWindowController.GenerateDefeateFriendMessage(targetName);
                while (_actionProcessor.IsPausedMessage)
                    yield return null;

                if (CharacterStatusManager.IsAllCharacterDefeated())
                {
                    _battleManager.OnGameover();
                }
            }
            else
            {
                _actionProcessor.SetPauseMessage(true);
                _battleSpriteController.HideEnemy();
                _messageWindowController.GenerateDefeateEnemyMessage(targetName);
                while (_actionProcessor.IsPausedMessage)
                    yield return null;

                if (_enemyStatusManager.IsAllEnemyDefeated())
                {
                    _battleManager.OnEnemyDefeated();
                }
            }

            _pauseMagicEffect = false;

            while (_actionProcessor.IsPausedMessage)
                yield return null;

            _pauseMagicEffect = false;
        }
       
        /// <summary>
        /// 魔法の対象が行動者の味方かどうかを判定します。
        /// </summary>
        /// <param name="magicEffect">魔法効果</param>
        bool IsMagicTargetFriend(MagicEffect magicEffect)
        {
            bool isFriend = false;
            if (magicEffect.effectTarget == EffectTarget.Own
                || magicEffect.effectTarget == EffectTarget.FriendSolo
                || magicEffect.effectTarget == EffectTarget.FriendAll)
            {
                isFriend = true;
            }
            return isFriend;
        }


        /// <summary>
        /// 魔法の効果がない場合のメッセージを表示します。
        /// </summary>
        IEnumerator ShowNoEffectMessage(BattleAction action, string magicName)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateMagicCastMessage(actorName, magicName);
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateNoEffectMessage();
            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            _pauseMagicEffect = false;
        }
    }
}