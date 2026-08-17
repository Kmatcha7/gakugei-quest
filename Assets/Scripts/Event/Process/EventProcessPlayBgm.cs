using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// BGMを再生するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessPlayBgm : EventProcessBase
    {
        /// <summary>
        /// 再生するBGMの名前です。
        /// </summary>
        [SerializeField]
        string _bgmName;

        /// <summary>
        /// 続きから再生するかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isResume = false;

        /// <summary>
        /// ループ再生するかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isLoop = true;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            AudioManager.Instance.PlayBgm(_bgmName, _isResume, _isLoop);
            CallNextProcess();
        }
    }
}