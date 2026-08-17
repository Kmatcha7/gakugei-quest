using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// フラグを操作するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessFlag : EventProcessBase
    {
        /// <summary>
        /// 対象のフラグ名です。
        /// </summary>
        [SerializeField]
        string _flagName;

        /// <summary>
        /// セットする状態です。
        /// </summary>
        [SerializeField]
        bool _flagState;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            FlagManager.Instance.SetFlagState(_flagName, _flagState);
            CallNextProcess();
        }
    }
}