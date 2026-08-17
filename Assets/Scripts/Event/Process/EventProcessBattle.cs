using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘機能を呼び出すイベントを処理するクラスです。
    /// </summary>
    public class EventProcessBattle : EventProcessBase, IPostBattle
    {
        /// <summary>
        /// 戦闘の対象の敵キャラクターIDです。
        /// </summary>
        [SerializeField]
        int _enemyId;

        /// <summary>
        /// 戦闘から逃げられるかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _canRunaway;

        /// <summary>
        /// 戦闘に負けた時に呼ばれるイベントの処理です。
        /// </summary>
        [SerializeField]
        EventProcessBase _loseProcess;

        /// <summary>
        /// この戦闘で使用するBGMの名前です。
        /// </summary>
        [SerializeField]
        string _battleBgmName = BgmNames.Battle;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            StartCoroutine(StartBattleProcess());
        }

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        IEnumerator StartBattleProcess()
        {
            // 現在のBGMを停止します。
            float fadeTime = 0.1f;
            AudioManager.Instance.StopAllBgm(fadeTime);

            var battleManager = FindAnyObjectByType<BattleManager>();
            if (battleManager == null)
            {
                SimpleLogger.Instance.LogError("シーン内のBattleManagerが見つかりませんでした。");
                CallNextProcess();
                yield break;
            }
            //魔法のリスト更新
            var statusSetter = FindAnyObjectByType<CharacterStatusSetter>();
            if (statusSetter != null)
            {
                statusSetter.UpdateMagicOnly();
            }
            // 戦闘開始の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.BattleStart);
            yield return new WaitForSeconds(fadeTime);

            battleManager.SetUpEnemyStatus(_enemyId);
            battleManager.SetCanRunaway(_canRunaway);
            battleManager.RegisterCallback(this);
            battleManager.StartBattle();

            // 戦闘BGMを再生します。
            AudioManager.Instance.PlayBgm(_battleBgmName, false, true);
        }

        /// <summary>
        /// 戦闘終了時のコールバックです。
        /// </summary>
        public void OnFinishedBattle()
        {
            // 現在のマップIDを取得します。
            var mapManager = FindAnyObjectByType<MapManager>();
            if (mapManager != null)
            {
                // BGMを続きから再生します。
                int mapId = mapManager.GetCurrentMapController().MapId;
                string bgmName = MapDataManager.GetMapBgmName(mapId);
                AudioManager.Instance.PlayBgm(bgmName, true, true);
            }

            CallNextProcess();
        }

        /// <summary>
        /// 戦闘で負けた時のコールバックです。
        /// </summary>
        public void OnLostBattle()
        {
            CallNextProcess(_loseProcess);
        }
    }
}