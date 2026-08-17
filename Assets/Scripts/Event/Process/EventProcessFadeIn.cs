using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 画面をフェードインさせるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessFadeIn : EventProcessBase, IFadeCallback
    {
        /// <summary>
        /// フェードにかかる時間です。
        /// </summary>
        [SerializeField]
        float _fadeTime = 0.5f;

        /// <summary>
        /// フェードの完了を待つかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isWaitFade = true;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            if (_isWaitFade)
            {
                FadeManager.Instance.SetCallback(this);
                FadeManager.Instance.FadeInScreen(_fadeTime);
            }
            else
            {
                FadeManager.Instance.SetCallback(null);
                FadeManager.Instance.FadeInScreen(_fadeTime);
                CallNextProcess();
            }
        }

        /// <summary>
        /// フェードが完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedFade()
        {
            CallNextProcess();
        }
    }
}