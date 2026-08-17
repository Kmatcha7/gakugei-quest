using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントのページを制御するクラスです。
    /// </summary>
    public class EventPage : MonoBehaviour
    {
        /// <summary>
        /// イベントのページの条件を保持するクラスのリストです。
        /// </summary>
        [SerializeField]
        List<EventPageConditionBase> _conditions;

        /// <summary>
        /// イベントのページのトリガーです。
        /// </summary>
        [SerializeField]
        RpgEventTrigger _eventTrigger;

        /// <summary>
        /// 最初に実行するイベントです。
        /// </summary>
        [SerializeField]
        EventProcessBase _startProcess;

        /// <summary>
        /// イベントの処理を行うクラスへの参照です。
        /// </summary>
        EventProcessor _eventProcessor;

        /// <summary>
        /// 実行対象のイベントです。
        /// </summary>
        EventProcessBase _targetEventProcess;

        /// <summary>
        /// 実行中のイベントデータです。
        /// </summary>
        EventFileData _eventFileData;

        /// <summary>
        /// 実行中のイベントデータです。
        /// </summary>
        public EventFileData EventFileData
        {
            get {return _eventFileData;}
        }

        /// <summary>
        /// イベントの条件を全て満たしているか確認します。
        /// </summary>
        public bool IsMatchedConditions()
        {
            foreach (var condition in _conditions)
            {
                // ひとつでも条件が満たされていない場合はfalseを返します。
                if (!condition.CheckCondition())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// イベントのトリガーが一致しているか確認します。
        /// </summary>
        public bool IsMatchedTrigger(RpgEventTrigger rpgEventTrigger)
        {
            return _eventTrigger == rpgEventTrigger;
        }

        /// <summary>
        /// イベントの処理を開始します。
        /// </summary>
        /// <param name="eventProcessor">イベントの処理を行うクラスへの参照です。</param>
        /// <param name="eventFileData">実行するイベントのデータ</param>
        public void StartEvent(EventProcessor eventProcessor, EventFileData eventFileData)
        {
            _eventProcessor = eventProcessor;
            _eventFileData = eventFileData;
            _targetEventProcess = _startProcess;

            ExecuteEventProcess();
        }

        /// <summary>
        /// 対象のイベントの処理を実行します。
        /// </summary>
        public void ExecuteEventProcess()
        {
            if (_targetEventProcess == null)
            {
                SimpleLogger.Instance.LogWarning($"実行対象のイベントプロセスがnullです。");
                OnFinishedEventPage();
                return;
            }

            _targetEventProcess.SetEventPageReference(this);
            _targetEventProcess.Execute();
        }

        /// <summary>
        /// イベントの個別の処理が完了した時の処理です。
        /// </summary>
        /// <param name="nextProcess">次に実行するイベント処理</param>
        public void OnFinishedEventProcess(EventProcessBase nextProcess)
        {
            if (nextProcess == null)
            {
                // 全てのイベント処理が完了した場合は、イベントページを終了します。
                OnFinishedEventPage();
                return;
            }

            _targetEventProcess = nextProcess;
            ExecuteEventProcess();
        }

        /// <summary>
        /// イベントページ内の処理が完了した時の処理です。
        /// </summary>
        public void OnFinishedEventPage()
        {
            _eventProcessor.OnEventFinished();
        }
    }
}