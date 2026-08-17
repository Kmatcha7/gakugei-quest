using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 指定したNPCの向きを変更するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessChangeNpcDirection : EventProcessBase
    {
        /// <summary>
        /// 対象のNPCのゲームオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _targetObject;

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

            characterMover.SetCharacterDirection(_targetDirection);
            CallNextProcess();
        }
    }
}
