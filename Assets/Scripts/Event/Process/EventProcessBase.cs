using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントの個別処理を行うクラスのベースクラスです。
    /// </summary>
    public class EventProcessBase : MonoBehaviour
    {
        /// <summary>
        /// 次に実行するイベントです。
        /// </summary>
        [SerializeField]
        protected EventProcessBase _nextProcess;

        /// <summary>
        /// イベントページへの参照です。
        /// </summary>
        protected EventPage _eventPage;

        /// <summary>
        /// イベントデータへの参照です。
        /// </summary>
        protected EventFileData _eventFileData;

        /// <summary>
        /// イベントページへの参照をセットします。
        /// </summary>
        /// <param name="eventPage">このプロセスが紐づくイベントページ</param>
        public virtual void SetEventPageReference(EventPage eventPage)
        {
            _eventPage = eventPage;
            if (_eventPage != null)
            {
                _eventFileData = eventPage.EventFileData;
            }
        }

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public virtual void Execute()
        {

        }

        /// <summary>
        /// 次のイベント処理を呼び出します。
        /// </summary>
        /// <param name="process">次に実行するイベント処理</param>
        public virtual void CallNextProcess(EventProcessBase process = null)
        {
            if (_eventPage == null)
            {
                SimpleLogger.Instance.LogWarning("イベントページへの参照が設定されていません。");
                return;
            }

            var targetProcess = process == null ? _nextProcess : process;
            SimpleLogger.Instance.Log($"{targetProcess?.name}を次のイベント処理として呼び出します。");
            _eventPage.OnFinishedEventProcess(targetProcess);
        }
    }
}