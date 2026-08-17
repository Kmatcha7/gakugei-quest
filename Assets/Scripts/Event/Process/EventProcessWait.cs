using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 待ち時間を入れるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessWait : EventProcessBase
    {
        /// <summary>
        /// イベントの待ち時間です。
        /// </summary>
        [SerializeField]
        float _waitTime = 1.0f;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            StartCoroutine(WaitProcess());
        }

        /// <summary>
        /// 指定した秒数だけ待機してから次の処理を呼び出します。
        /// </summary>
        IEnumerator WaitProcess()
        {
            yield return new WaitForSeconds(_waitTime);
            CallNextProcess();
        }
    }
}