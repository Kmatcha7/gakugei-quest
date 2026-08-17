using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターの移動に関するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessMovePlayer : EventProcessBase, ICharacterMoveCallback
    {
        /// <summary>
        /// 移動する方向です。
        /// </summary>
        [SerializeField]
        MoveAnimationDirection _targetDirection;

        /// <summary>
        /// 移動する歩数です。
        /// </summary>
        [SerializeField]
        int _moveSteps;

        /// <summary>
        /// 移動の完了を待つかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isWaitMove = true;

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
            playerMover.ForceMoveCharacter(_targetDirection, _moveSteps, true, this);

            if (!_isWaitMove)
            {
                // 移動を待たない場合は次のプロセスを呼び出します。
                CallNextProcess();
            }
        }

        /// <summary>
        /// キャラクターの移動が完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedMove()
        {
            if (!_isWaitMove)
            {
                return;
            }
            CallNextProcess();
        }
    }
}