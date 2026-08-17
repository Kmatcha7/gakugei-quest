using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントのページでフラグに関する条件を確認するクラスです。
    /// </summary>
    public class EventPageConditionFlag : EventPageConditionBase
    {
        /// <summary>
        /// 条件を確認するフラグ名です。
        /// </summary>
        [SerializeField]
        string _flagName;

        /// <summary>
        /// フラグの状態が以下の値に一致するかどうかを確認します。
        /// </summary>
        [SerializeField]
        bool _state;

        /// <summary>
        /// 条件を確認します。
        /// </summary>
        /// <returns>条件が満たされている場合はtrue、そうでない場合はfalseを返します。</returns>
        public override bool CheckCondition()
        {
            var currentState = FlagManager.Instance.GetFlagState(_flagName);
            return _state == currentState;
        }
    }
}