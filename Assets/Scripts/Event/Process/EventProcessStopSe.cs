using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 効果音を停止させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessStopSe : EventProcessBase
    {
        /// <summary>
        /// フェードにかかる時間です。
        /// </summary>
        [SerializeField]
        float _fadeTime = 0.25f;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            AudioManager.Instance.StopAllSe(_fadeTime);
            CallNextProcess();
        }
    }
}