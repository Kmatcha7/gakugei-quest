using UnityEngine;
using System.Linq;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターとのエンカウントを管理するクラスです。
    /// </summary>
    public class EncounterManager : MonoBehaviour, IPostBattle
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleManager _battleManager;

        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// イベントの処理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        EventProcessor _eventProcessor;

        /// <summary>
        /// 戦闘に負けた時に実行するイベントデータです。
        /// </summary>
        [SerializeField]
        EventFileData _loseEvent;

        /// <summary>
        /// 戦闘で使用するBGMの名前です。
        /// </summary>
        [SerializeField]
        string _battleBgmName = BgmNames.Battle;

        /// <summary>
        /// 現在のマップのエンカウント情報です。
        /// </summary>
        EncounterData _currentEncounterData;

        /// <summary>
        /// 現在のマップのエンカウント情報をセットします。
        /// </summary>
        /// <param name="encounterData">エンカウントデータ</param>
        public void SetCurrentEncounterData(EncounterData encounterData)
        {
            _currentEncounterData = encounterData;
        }

        /// <summary>
        /// エンカウントが発生するかどうか確認します。
        /// </summary>
        public void CheckEncounter()
        {
            if (!IsHappenEncounter())
            {
                // エンカウントが発生しない場合は処理を抜けます。
                return;
            }

            // 戦闘を開始します。
            int enemyId = GetEncounterEnemyId();
            var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);
            if (enemyData == null)
            {
                // エンカウントする敵キャラクターのデータが取得できない場合は処理を抜けます。
                return;
            }

            // エンカウントが発生する場合は、キャラクターの移動を停止します。
            _characterMoverManager.StopCharacterMover();
            StartCoroutine(StartBattleProcess(enemyId));
        }

        /// <summary>
        /// エンカウントが発生するかどうか確認します。
        /// </summary>
        IEnumerator StartBattleProcess(int enemyId)
        {
             //魔法のリスト更新
            var statusSetter = FindAnyObjectByType<CharacterStatusSetter>();
            if (statusSetter != null)
            {
                statusSetter.UpdateMagicOnly();
            }
            // 現在のBGMを停止します。
            float fadeTime = 0.1f;
            AudioManager.Instance.StopAllBgm(fadeTime);

            // 戦闘開始の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.BattleStart);
            yield return new WaitForSeconds(fadeTime);

            _battleManager.SetUpEnemyStatus(enemyId);
            _battleManager.SetCanRunaway(true);
            _battleManager.RegisterCallback(this);
            _battleManager.StartBattle();
            AudioManager.Instance.PlayBgm(_battleBgmName, false, true);
        }

        /// <summary>
        /// エンカウントが発生する場合にTrueを返します。
        /// </summary>
        bool IsHappenEncounter()
        {
            if (_currentEncounterData == null)
            {
                // エンカウントデータがセットされていない場合はエンカウントしません。
                return false;
            }

            if (_currentEncounterData.enemyRates == null)
            {
                // エンカウントする敵キャラクターのデータがセットされていない場合はエンカウントしません。
                return false;
            }
            
            if (!_currentEncounterData.hasEncounter)
            {
                // エンカウントフラグがfalseの場合はエンカウントしません。
                return false;
            }

            // エンカウントの確率を取得します。
            float rate = _currentEncounterData.rate;
            float random = Random.Range(0f, 100f);
            SimpleLogger.Instance.Log($"エンカウントの確率: {rate} 乱数: {random} エンカウントするかどうか: {random <= rate}");
            if (random > rate)
            {
                // 乱数の値がエンカウントの発生率より高い場合はエンカウントしません。
                return false;
            }

            // 全ての敵キャラクターの出現率が0の場合もエンカウントしません。
            int nonZeroEnemy = _currentEncounterData.enemyRates.Count(r => r.rate > 0);
            if (nonZeroEnemy == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// エンカウントする対象の敵キャラクターのIDを取得します。
        /// </summary>
        int GetEncounterEnemyId()
        {
            int enemyId= 0;

            // 出現率の合計値を計算します。
            int sumProbability = 0;
            foreach (var enemyRate in _currentEncounterData.enemyRates)
            {
                sumProbability += enemyRate.rate;
            }

            // 乱数を生成して、出現率の合計値の範囲内で敵キャラクターを選択します。
            int random = Random.Range(0, sumProbability);
            foreach (var enemyRate in _currentEncounterData.enemyRates)
            {
                random -= enemyRate.rate;
                if (random < 0)
                {
                    enemyId = enemyRate.enemyId;
                    break;
                }
            }

            return enemyId;
        }

        /// <summary>
        /// 戦闘終了時のコールバックです。
        /// </summary>
        public void OnFinishedBattle()
        {
            // キャラクターが移動できるようにします。
            _characterMoverManager.ResumeCharacterMover();
            GameStateManager.ChangeToMoving();

            // 現在のマップIDを取得します。
            var mapManager = FindAnyObjectByType<MapManager>();
            if (mapManager != null)
            {
                // BGMを続きから再生します。
                int mapId = mapManager.GetCurrentMapController().MapId;
                string bgmName = MapDataManager.GetMapBgmName(mapId);
                AudioManager.Instance.PlayBgm(bgmName, true, true);
            }
        }

        /// <summary>
        /// 戦闘で負けた時のコールバックです。
        /// </summary>
        public void OnLostBattle()
        {
            // 戦闘に負けた場合は村長の前に戻します。
            _eventProcessor.ExecuteEvent(_loseEvent, RpgEventTrigger.System, null);
        }
    }
}