using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// BGMを停止させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessStopBgm : EventProcessBase
    {
        /// <summary>
        /// フェードにかかる時間です。
        /// </summary>
        [SerializeField]
        float _fadeTime = 0.25f;

        /// <summary>
        /// 全体を停止させるかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isStopAll = true;

        /// <summary>
        /// 停止するBGMの名前です。
        /// </summary>
        [SerializeField]
        string _bgmName;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            if (_isStopAll)
            {
                AudioManager.Instance.StopAllBgm(_fadeTime);
            }
            else
            {
                AudioManager.Instance.StopBgm(_bgmName, _fadeTime);
            }
            CallNextProcess();
        }
    }
}