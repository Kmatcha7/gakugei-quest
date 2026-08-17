using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントの処理を行うクラスです。
    /// </summary>
    public class EventProcessor : MonoBehaviour
    {
        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// 現在処理中のイベントです。
        /// </summary>
        EventFileData _currentEventFileData;

        /// <summary>
        /// イベント完了後のコールバック先です。
        /// </summary>
        IEventCallback _callback;

        /// <summary>
        /// 実行するイベントのキューです。
        /// </summary>
        Queue<EventQueue> _eventQueue = new();

        /// <summary>
        /// イベントが実行中かどうかのフラグです。
        /// </summary>
        bool _isEventRunning = false;

        /// <summary>
        /// キューにイベントを追加します。
        /// </summary>
        /// <param name="eventQueue">追加するイベントキュー</param>
        public void AddQueue(EventQueue eventQueue)
        {
            _eventQueue.Enqueue(eventQueue);
        }

        /// <summary>
        /// キューに追加されたイベントを開始します。
        /// </summary>
        public void StartEvent()
        {
            StartCoroutine(StartEventProcess());
        }

        /// <summary>
        /// キューに追加されたイベントを開始する処理です。
        /// </summary>
        IEnumerator StartEventProcess()
        {
            if (_isEventRunning)
            {
                SimpleLogger.Instance.Log("イベントが既に実行中のため、完了を待ちます。");
                while (_isEventRunning)
                {
                    yield return null; // イベントが完了するまで待機
                }
            }

            if (_eventQueue.Count == 0)
            {
                SimpleLogger.Instance.Log("イベントキューが空のため処理を抜けます。");
                yield break;
            }

            var eventQueue = _eventQueue.Dequeue();
            if (eventQueue == null)
            {
                SimpleLogger.Instance.Log("イベントキューが空です。");
                yield break;
            }
            ExecuteEvent(eventQueue.targetObj, eventQueue.rpgEventTrigger, eventQueue.callback);
        }

        /// <summary>
        /// イベントの実行を開始します。
        /// </summary>
        /// <param name="targetObj">イベントを確認する対象のゲームオブジェクト</param>
        /// <param name="rpgEventTrigger">イベントのトリガー</param>
        /// <param name="callback">イベント完了後のコールバック先</param>
        public void ExecuteEvent(GameObject targetObj, RpgEventTrigger rpgEventTrigger, IEventCallback callback)
        {
            _callback = callback;
            var eventFileData = GetEventFile(targetObj);
            ExecuteEventFile(eventFileData, rpgEventTrigger);
        }

        /// <summary>
        /// イベントの実行を開始します。
        /// </summary>
        /// <param name="eventFileData">イベントファイルデータ</param>
        /// <param name="rpgEventTrigger">イベントのトリガー</param>
        /// <param name="callback">イベント完了後のコールバック先</param>
        public void ExecuteEvent(EventFileData eventFileData, RpgEventTrigger rpgEventTrigger, IEventCallback callback)
        {
            _callback = callback;
            ExecuteEventFile(eventFileData, rpgEventTrigger);
        }

        /// <summary>
        /// 引数のゲームオブジェクトについて、実行するイベントがあるか確認します。
        /// </summary>
        /// <param name="targetObj">イベントを確認する対象のゲームオブジェクト</param>
        EventFileData GetEventFile(GameObject targetObj)
        {
            var eventFileData = targetObj.GetComponent<EventFileData>();
            if (eventFileData == null)
            {
                eventFileData = targetObj.GetComponentInChildren<EventFileData>();
                if (eventFileData == null)
                {
                    SimpleLogger.Instance.LogWarning($"イベントファイルデータが見つかりませんでした。対象のゲームオブジェクト : {targetObj.name}");
                    return null;
                }
            }

            return eventFileData;
        }

        /// <summary>
        /// 引数のゲームオブジェクトについて、実行するイベントがあるか確認します。
        /// </summary>
        /// <param name="targetObj">イベントを確認する対象のゲームオブジェクト</param>
        public void ExecuteEventFile(EventFileData eventFileData, RpgEventTrigger rpgEventTrigger)
        {
            if (eventFileData == null)
            {
                SimpleLogger.Instance.LogWarning($"イベントファイルデータが見つかりませんでした。");
                return;
            }

            _isEventRunning = true;
            GameStateManager.ChangeToEvent();
            _characterMoverManager.StopCharacterMover();
            _currentEventFileData = eventFileData;
            _currentEventFileData.SetReference(this);
            _currentEventFileData.ExecuteEvent(rpgEventTrigger);
        }

        /// <summary>
        /// イベントの処理が終了した場合のコールバックです。
        /// </summary>
        public void OnEventFinished()
        {
            _currentEventFileData = null;
            _isEventRunning = false;

            if (_callback != null)
            {
                _callback.OnFinishedEvent();
            }
            _callback = null;

            if (_eventQueue.Count > 0)
            {
                // キューにイベントが残っている場合は次のイベントを開始します。
                StartEvent();
                return;
            }

            // すべてのイベントが完了した場合は、ゲーム状態を移動に戻します。
            GameStateManager.ChangeToMoving();
            _characterMoverManager.ResumeCharacterMover();
        }
    }
}