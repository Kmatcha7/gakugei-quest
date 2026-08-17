using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントのページの条件を保持するクラスのベースクラスです。
    /// </summary>
    public class EventPageConditionBase : MonoBehaviour
    {
        /// <summary>
        /// 条件を確認します。
        /// </summary>
        /// <returns>条件が満たされている場合はtrue、そうでない場合はfalseを返します。</returns>
        public virtual bool CheckCondition()
        {
            return true;
        }
    }
}