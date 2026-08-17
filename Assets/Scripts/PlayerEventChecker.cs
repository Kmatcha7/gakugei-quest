using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 操作キャラの移動に伴うイベントの確認を行うクラスです。
    /// </summary>
    public class PlayerEventChecker : MonoBehaviour, IEventCallback
    {
        /// <summary>
        /// 操作キャラの移動制御を行うクラスへの参照です。
        /// </summary>
        PlayerMover _playerMover;

        /// <summary>
        /// イベントの処理を行うクラスへの参照です。
        /// </summary>
        EventProcessor _eventProcessor;

        /// <summary>
        /// Tilemapを管理するクラスへの参照です。
        /// </summary>
        TilemapManager _tilemapManager;

        /// <summary>
        /// イベントを実行しているキャラクターの移動用クラスへの参照です。
        /// </summary>
        CharacterMover _eventTargetMover;

        /// <summary>
        /// 移動するキャラクターのColliderです。
        /// </summary>
        BoxCollider2D _boxCollider2d;

        /// <summary>
        /// 参照をセットします。
        /// </summary>
        /// <param name="playerMover">操作キャラの移動制御を行うクラス</param>
        public void SetUpReference(PlayerMover playerMover)
        {
            _playerMover = playerMover;

            _boxCollider2d = _playerMover.GetComponent<BoxCollider2D>();

            if (_eventProcessor == null)
            {
                _eventProcessor = FindAnyObjectByType<EventProcessor>();
            }

            if (_tilemapManager == null)
            {
                _tilemapManager = FindAnyObjectByType<TilemapManager>();
            }
        }

        /// <summary>
        /// イベントを開始するキー入力を確認します。
        /// </summary>
        public void CheckEventInput()
        {
            // 移動フェーズ以外は処理を抜けます。
            if (GameStateManager.CurrentState != GameState.Moving)
            {
                return;
            }

            // このキャラクターが移動中の場合、または移動が停止している場合は処理を抜けます。
            if (_playerMover.IsMoving || _playerMover.IsMovingPaused)
            {
                return;
            }

            // イベントの開始キーが押された場合は、イベントを開始します。
            if (InputGameKey.ConfirmButton())
            {
                CheckEvent();
            }
        }

        /// <summary>
        /// イベントが終了した時に呼ばれるコールバックです。
        /// </summary>
        public void OnFinishedEvent()
        {
            SetEventMoverDirectionPrevious();
            _eventTargetMover = null;
        }

        /// <summary>
        /// イベントを確認する処理です。
        /// </summary>
        public void CheckEvent()
        {
            // 目の前にいるゲームオブジェクトを確認します。
            var moveDirection = _playerMover.GetMoveDirection(_playerMover.AnimationDirection);
            var raycastHits = _playerMover.GetOtherCharacter(moveDirection);

            // 対象のゲームオブジェクトを取得します。
            var hitObj = GetGameObjectFromRaycastHits(raycastHits);
            if (hitObj == null)
            {
                return;
            }

            // 移動用の制御クラスがアタッチされている場合はキャッシュします。
            _eventTargetMover = hitObj.GetComponent<CharacterMover>();

            // イベントファイルをイベント処理のクラスに渡します。
            StartEvent(hitObj, RpgEventTrigger.ConfirmButton);
        }

        /// <summary>
        /// 引数のレイキャストの結果からゲームオブジェクトを取得します。
        /// </summary>
        /// <param name="raycastHits">レイキャストの結果のリスト</param>
        GameObject GetGameObjectFromRaycastHits(List<RaycastHit2D> raycastHits)
        {
            if (raycastHits == null || raycastHits.Count == 0)
            {
                return null;
            }

            GameObject targetObj = raycastHits[0].transform.gameObject;
            return targetObj;
        }

        /// <summary>
        /// 引数のColliderのリストからゲームオブジェクトを取得します。
        /// </summary>
        /// <param name="colliders">Collider2Dのリスト</param>
        GameObject GetGameObjectFromColliders(List<Collider2D> colliders)
        {
            if (colliders == null || colliders.Count == 0)
            {
                return null;
            }

            // 最初のColliderのゲームオブジェクトを返します。
            GameObject targetObj = colliders[0].gameObject;
            return targetObj;
        }

        /// <summary>
        /// イベントの実行対象となるキャラクターの向きを変更します。
        /// </summary>
        void SetEventMoverDirectionToPlayer()
        {
            if (_eventTargetMover == null)
            {
                SimpleLogger.Instance.Log($"イベントの対象となるキャラクターの移動クラスが見つかりませんでした。");
                return;
            }

            // 主人公の向きの反対を取得します。
            int playerDirection = (int)_playerMover.AnimationDirection;
            int oppsiteDirection = (playerDirection + 2) % 4;

            // イベントの対象となるキャラクターの向きを設定します。
            _eventTargetMover.SetCharacterDirection((MoveAnimationDirection)oppsiteDirection);
        }

        /// <summary>
        /// イベントの実行対象となるキャラクターの向きを変更します。
        /// </summary>
        void SetEventMoverDirectionPrevious()
        {
            if (_eventTargetMover == null)
            {
                SimpleLogger.Instance.Log($"イベントの対象となるキャラクターの移動クラスが見つかりませんでした。");
                return;
            }

            // 元の向きに設定します。
            _eventTargetMover.SetCharacterDirection(_eventTargetMover.AnimationDirection);
        }

        /// <summary>
        /// タイル上で重なったイベントを確認します。
        /// </summary>
        public bool CheckOnTileEvent()
        {
            // 重なったゲームオブジェクトを確認します。
            List<Collider2D> colliders = new List<Collider2D>();
            _boxCollider2d.Overlap(colliders);

            // 対象のゲームオブジェクトを取得します。
            var eventObj = GetGameObjectFromColliders(colliders);
            if (eventObj == null)
            {
                return false;
            }

            // 対象のゲームオブジェクトのマップ上の位置を確認します。
            var eventPos = _tilemapManager.GetPositionOnTilemap(eventObj.transform.position);
            if (eventPos != _playerMover.PosOnTile)
            {
                return false;
            }

            // イベントファイルをイベント処理のクラスに渡します。
            StartEvent(eventObj, RpgEventTrigger.OnTile);
            return true;
        }

        /// <summary>
        /// イベントの処理を開始します。
        /// </summary>
        /// <param name="eventObj">イベントを実行する対象のゲームオブジェクト</param>
        /// <param name="rpgEventTrigger">イベントのトリガー</param>
        void StartEvent(GameObject eventObj, RpgEventTrigger rpgEventTrigger)
        {
            // イベントファイルをイベント処理のクラスに渡します。
            if (_eventProcessor == null)
            {
                SimpleLogger.Instance.LogError("EventProcessorが見つかりませんでした。");
                return;
            }

            // イベントの対象となるキャラクターの向きを設定します。
            SetEventMoverDirectionToPlayer();

            // イベントの開始を通知します。
            var eventQueue = new EventQueue
            {
                targetObj = eventObj,
                rpgEventTrigger = rpgEventTrigger,
                callback = this
            };
            _eventProcessor.AddQueue(eventQueue);
            _eventProcessor.StartEvent();
        }
    }
}