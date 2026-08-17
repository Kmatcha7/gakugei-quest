using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 指定したNPCを移動させるイベント処理クラスです。
    /// </summary>
    public class EventProcessMoveNpc : EventProcessBase, ICharacterMoveCallback
    {
        /// <summary>
        /// 対象のNPCのゲームオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _targetObject;

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
            SimpleLogger.Instance.Log($"EventProcessMoveNpc.Execute target:{_targetObject?.name} steps:{_moveSteps} direction:{_targetDirection} isWait:{_isWaitMove}");

            if (_targetObject == null)
            {
                SimpleLogger.Instance.LogError("対象のNPCが設定されていません。");
                CallNextProcess();
                return;
            }

            var characterMover = _targetObject.GetComponent<CharacterMover>();
            if (characterMover == null)
            {
                SimpleLogger.Instance.LogError($"対象オブジェクトに CharacterMover コンポーネントがありません。");
                CallNextProcess();
                return;
            }

            characterMover.ForceMoveCharacter(_targetDirection, _moveSteps, true, this);

            SimpleLogger.Instance.Log($"EventProcessMoveNpc: StartMove target:{_targetObject?.name} steps:{_moveSteps} direction:{_targetDirection} isWait:{_isWaitMove}");

            if (!_isWaitMove)
            {
                CallNextProcess();
            }
        }

        void Awake()
        {
            SimpleLogger.Instance.Log($"EventProcessMoveNpc.Awake name:{gameObject.name} active:{gameObject.activeInHierarchy} enabled:{enabled} instanceId:{GetInstanceID()}");
        }

        void OnEnable()
        {
            SimpleLogger.Instance.Log($"EventProcessMoveNpc.OnEnable name:{gameObject.name} active:{gameObject.activeInHierarchy} enabled:{enabled} instanceId:{GetInstanceID()}");
        }

        /// <summary>
        /// キャラクターの移動が完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedMove()
        {
            SimpleLogger.Instance.Log($"EventProcessMoveNpc: FinishedMove target:{_targetObject?.name} steps:{_moveSteps} direction:{_targetDirection}");

            if (!_isWaitMove)
            {
                return;
            }
            CallNextProcess();
        }
    }
}
