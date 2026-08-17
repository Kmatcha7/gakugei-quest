using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントのまとまりを制御するクラスです。
    /// </summary>
    public class EventFileData : MonoBehaviour
    {
        /// <summary>
        /// 条件に応じたイベントの画像を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        EventGraphicController _eventGraphicController;

        /// <summary>
        /// イベントページのリストです。
        /// </summary>
        List<EventPage> _eventPages;

        /// <summary>
        /// イベントの処理を行うクラスへの参照です。
        /// </summary>
        EventProcessor _eventProcessor;

        /// <summary>
        /// イベントを処理するクラスへの参照をセットします。
        /// </summary>
        public void SetReference(EventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }

        /// <summary>
        /// 対象のイベントページの処理を開始します。
        /// </summary>
        public void ExecuteEvent(RpgEventTrigger rpgEventTrigger)
        {
            SetUpEventPages();
            var targetPage = GetEventPage(rpgEventTrigger);
            if (targetPage == null)
            {
                SimpleLogger.Instance.Log($"実行可能なイベントページが見つかりませんでした。イベントのトリガー : {rpgEventTrigger}");
                _eventProcessor.OnEventFinished();
                return;
            }

            targetPage.StartEvent(_eventProcessor, this);
        }

        /// <summary>
        /// イベントページのリストをセットアップします。
        /// </summary>
        void SetUpEventPages()
        {
            if (_eventPages == null)
            {
                var pages = GetComponentsInChildren<EventPage>();
                _eventPages = new List<EventPage>(pages);
            }
        }

        /// <summary>
        /// 実行対象のイベントページを取得します。
        /// </summary>
        EventPage GetEventPage(RpgEventTrigger rpgEventTrigger)
        {
            EventPage targetPage = null;

            // Hierarchyウィンドウで下から順にイベントページを確認し、条件に合致する最初のページを返します。
            for (int i = _eventPages.Count - 1; i >= 0; i--)
            {
                var page = _eventPages[i];
                if (page == null)
                {
                    SimpleLogger.Instance.LogWarning("イベントページがnullです。");
                    continue;
                }

                if (page.IsMatchedConditions() && page.IsMatchedTrigger(rpgEventTrigger))
                {
                    targetPage = page;
                    break;
                }
            }
            return targetPage;
        }

        /// <summary>
        /// 条件が合致するイベントページを取得します。
        /// </summary>
        EventPage GetEventPage()
        {
            EventPage targetPage = null;

            // Hierarchyウィンドウで下から順にイベントページを確認し、条件に合致する最初のページを返します。
            for (int i = _eventPages.Count - 1; i >= 0; i--)
            {
                var page = _eventPages[i];
                if (page == null)
                {
                    SimpleLogger.Instance.LogWarning("イベントページがnullです。");
                    continue;
                }

                if (page.IsMatchedConditions())
                {
                    targetPage = page;
                    break;
                }
            }
            return targetPage;
        }

        /// <summary>
        /// イベントオブジェクトのグラフィックを設定します。
        /// </summary>
        public void SetEventGraphic()
        {
            SetUpEventPages();
            var targetPage = GetEventPage();
            if (targetPage == null)
            {
                return;
            }

            if (_eventGraphicController != null)
            {
                _eventGraphicController.SetEventGraphic(targetPage);
            }
        }
    }
}