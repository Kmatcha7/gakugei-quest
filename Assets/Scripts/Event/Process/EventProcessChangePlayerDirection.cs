using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 主人公の向きを変更するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessChangePlayerDirection : EventProcessBase
    {
        /// <summary>
        /// 変更する方向です。
        /// </summary>
        [SerializeField]
        MoveAnimationDirection _targetDirection;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            // 主人公のゲームオブジェクトを取得します。
            var playerMover = FindAnyObjectByType<PlayerMover>();
            if (playerMover == null)
            {
                SimpleLogger.Instance.LogError("PlayerMoverが見つかりません。");
                CallNextProcess();
                return;
            }
            playerMover.SetCharacterDirection(_targetDirection);

            CallNextProcess();
        }
    }
}