using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 効果音を再生するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessPlaySe : EventProcessBase
    {
        /// <summary>
        /// 再生する効果音の名前です。
        /// </summary>
        [SerializeField]
        string _seName;

        /// <summary>
        /// ループ再生するかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _isLoop = false;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            AudioManager.Instance.PlaySe(_seName, _isLoop);
            CallNextProcess();
        }
    }
}