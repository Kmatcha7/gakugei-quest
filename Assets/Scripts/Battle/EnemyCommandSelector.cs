using System.Linq;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターのコマンドを選択するクラスです。
    /// </summary>
    public class EnemyCommandSelector : MonoBehaviour
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘中の敵キャラクターのデータを管理するクラスへの参照です。
        /// </summary>
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// 戦闘中のアクションを登録するクラスへの参照です。
        /// </summary>
        BattleActionRegister _battleActionRegister;

        /// <summary>
        /// 参照をセットします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラスへの参照</param>
        /// <param name="battleActionRegister">戦闘中のアクションを登録するクラスへの参照</param>
        public void SetReferences(BattleManager battleManager, BattleActionRegister battleActionRegister)
        {
            _battleManager = battleManager;
            _battleActionRegister = battleActionRegister;
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        /// <summary>
        /// 敵キャラクターのコマンドを選択します。
        /// </summary>
        public void SelectEnemyCommand()
        {
            foreach (var enemyStatus in _enemyStatusManager.GetEnemyStatusList())
            {
                if (enemyStatus.isDefeated || enemyStatus.isRunaway)
                {
                    continue;
                }

                // 先頭のパーティキャラクターをターゲットにします。
                int targetId = CharacterStatusManager.partyCharacter[0];

                // 行動パターンに応じて敵キャラクターのコマンドを選択します。
                EnemyActionRecord record = SelectActionFromRecords(enemyStatus.enemyData, enemyStatus.enemyBattleId);
                if (record == null)
                {
                    _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                    continue;
                }

                switch (record.enemyActionCategory)
                {
                    case EnemyActionCategory.Attack:
                        _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                        break;
                    case EnemyActionCategory.Magic:
                        if (CanUseMagic(record, enemyStatus.enemyBattleId))
                        {
                            _battleActionRegister.SetEnemyMagicAction(enemyStatus.enemyBattleId, targetId, record.magicData.magicId, enemyStatus.enemyData);
                        }
                        else
                        {
                            _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                        }
                        break;
                    default:
                        _battleActionRegister.SetEnemyAttackAction(enemyStatus.enemyBattleId, targetId, enemyStatus.enemyData);
                        break;
                }
            }

            // 敵キャラクターのコマンド選択が完了したことを通知します。
            _battleManager.OnEnemyCommandSelected();
        }

        /// <summary>
        /// 行動パターンを選択します。
        /// </summary>
        /// <param name="enemyData">敵キャラクターのデータ</param>
        /// <param name="enemyBattleId">敵キャラクターの戦闘中ID</param>
        EnemyActionRecord SelectActionFromRecords(EnemyData enemyData, int enemyBattleId)
        {
            var availableActions = enemyData.enemyActionRecords
                .Where(r => CheckCondition(r, enemyBattleId))
                .ToList();

            if (availableActions.Count == 0)
                return null;

            int totalWeight = availableActions.Sum(a => a.priority);
            int randomValue = Random.Range(0, totalWeight);

            int current = 0;

            foreach (var action in availableActions)
            {
                current += action.priority;

                if (randomValue < current)
                {
                    return action;
                }
            }

            return availableActions[0];
        }

        /// <summary>
        /// 行動パターンの条件に合致しているか確認します。
        /// Trueで合致しています。
        /// </summary>
        /// <param name="conditionRecords">条件のデータ</param>
        /// <param name="enemyBattleId">敵キャラクターの戦闘中ID</param>
        bool CheckCondition(EnemyActionRecord record, int enemyBattleId)
        {
            // 条件が無い場合は常に実行
            if (record.enemyConditionRecords == null || record.enemyConditionRecords.Count == 0)
            {
                return true;
            }

            bool match = false;

            foreach (var conditionRecord in record.enemyConditionRecords)
            {
                switch (conditionRecord.conditionCategory)
                {
                    case ConditionCategory.Turn:
                        match = CheckTurnCondition(conditionRecord);
                        break;

                    case ConditionCategory.HpRate:
                        match = CheckHpRateCondition(conditionRecord, enemyBattleId);
                        break;
                }

                if (match) return true;
            }

            return false;
        }

        /// <summary>
        /// 行動パターンの条件に合致しているか確認します。
        /// Trueで合致しています。
        /// </summary>
        /// <param name="conditionRecords">条件のデータ</param>
        bool CheckTurnCondition(EnemyConditionRecord record)
        {
            return CompareValues(_battleManager.TurnCount, record.comparisonOperator, record.turnCriteria);
        }

        /// <summary>
        /// HP残量の条件に合致しているか確認します。
        /// Trueで合致しています。
        /// </summary>
        /// <param name="conditionRecords">条件のデータ</param>
        /// <param name="enemyBattleId">敵キャラクターの戦闘中ID</param>
        bool CheckHpRateCondition(EnemyConditionRecord record, int enemyBattleId)
        {
            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            int currentHp = enemyStatus.currentHp;
            int maxHp = enemyStatus.enemyData.hp;
            float hpRate = currentHp * 100f / maxHp;
            SimpleLogger.Instance.Log($"HP残量の条件を確認します。 currentHp : {currentHp} || maxHp : {maxHp} || HP残量 : {hpRate}");
            return CompareValues(hpRate, record.comparisonOperator, record.hpRateCriteria);
        }

        /// <summary>
        /// 選択された魔法が使用可能か確認します。
        /// Trueで使用可能です。
        /// </summary>
        /// <param name="record">行動パターンのデータ</param>
        /// <param name="enemyBattleId">敵キャラクターの戦闘中ID</param>
        bool CanUseMagic(EnemyActionRecord record, int enemyBattleId)
        {
            // 魔法データがnullの場合はfalseを返します。
            if (record.magicData == null)
            {
                return false;
            }

            var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(enemyBattleId);
            int currentMp = enemyStatus.currentMp;
            int mpCost = record.magicData.cost;
            bool canUse = currentMp >= mpCost;
            SimpleLogger.Instance.Log($"{record.magicData.magicName} の魔法が使用できるか確認します。 currentMp : {currentMp} mpCost : {mpCost} canUse : {canUse}");
            return canUse;
        }

        /// <summary>
        /// 演算子に応じた条件を満たしているか確認します。
        /// </summary>
        /// <param name="targetValue">対象の値</param>
        /// <param name="comparisonOperator">演算子</param>
        /// <param name="criteria">条件の値</param>
        bool CompareValues(int targetValue, ComparisonOperator comparisonOperator, int criteria)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals:
                    return targetValue == criteria;
                case ComparisonOperator.NotEquals:
                    return targetValue != criteria;
                case ComparisonOperator.GreaterThan:
                    return targetValue > criteria;
                case ComparisonOperator.GreaterOrEqual:
                    return targetValue >= criteria;
                case ComparisonOperator.LessThan:
                    return targetValue < criteria;
                case ComparisonOperator.LessOrEqual:
                    return targetValue <= criteria;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 演算子に応じた条件を満たしているか確認します。
        /// </summary>
        /// <param name="targetValue">対象の値</param>
        /// <param name="comparisonOperator">演算子</param>
        /// <param name="criteria">条件の値</param>
        bool CompareValues(float targetValue, ComparisonOperator comparisonOperator, float criteria)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals:
                    return targetValue == criteria;
                case ComparisonOperator.NotEquals:
                    return targetValue != criteria;
                case ComparisonOperator.GreaterThan:
                    return targetValue > criteria;
                case ComparisonOperator.GreaterOrEqual:
                    return targetValue >= criteria;
                case ComparisonOperator.LessThan:
                    return targetValue < criteria;
                case ComparisonOperator.LessOrEqual:
                    return targetValue <= criteria;
                default:
                    return false;
            }
        }
    }
}