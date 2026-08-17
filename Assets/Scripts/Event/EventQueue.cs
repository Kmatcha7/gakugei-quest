using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントのキューの情報を保持するクラスです。
    /// </summary>
    public class EventQueue
    {
        /// <summary>
        /// イベントの情報を保持するゲームオブジェクトです。
        /// </summary>
        public GameObject targetObj;

        /// <summary>
        /// イベントの実行方法です。
        /// </summary>
        public RpgEventTrigger rpgEventTrigger;

        /// <summary>
        /// イベント完了後のコールバック先です。
        /// </summary>
        public IEventCallback callback;
    }
}