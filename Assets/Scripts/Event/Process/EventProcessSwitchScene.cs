using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleRpg
{
    /// <summary>
    /// シーンを遷移させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessSwitchScene : EventProcessBase
    {
        /// <summary>
        /// 遷移先のシーン名です。
        /// </summary>
        [SerializeField]
        string _targetScene = "Title";

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            SceneManager.LoadScene(_targetScene);
            CallNextProcess();
        }
    }
}