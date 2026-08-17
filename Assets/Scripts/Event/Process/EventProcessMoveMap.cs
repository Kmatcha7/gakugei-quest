using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 場所移動に関するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessMoveMap : EventProcessBase
    {
        /// <summary>
        /// 移動先のマップIDです。
        /// </summary>
        [SerializeField]
        int _targetMapId;

        /// <summary>
        /// 移動先の座標です。
        /// </summary>
        [SerializeField]
        Vector3Int _targetPosition;

        /// <summary>
        /// マップ機能を管理するクラスへの参照です。
        /// </summary>
        MapManager _mapManager;

        /// <summary>
        /// 操作キャラの移動制御を行うクラスへの参照です。
        /// </summary>
        PlayerMover _playerMover;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            SetUpReference();

            if (_mapManager == null)
            {
                SimpleLogger.Instance.LogError("MapManagerが見つかりません。");
                CallNextProcess();
                return;
            }

            if (_playerMover == null)
            {
                SimpleLogger.Instance.LogError("PlayerMoverが見つかりません。");
                CallNextProcess();
                return;
            }

            _mapManager.ShowMap(_targetMapId);
            _playerMover.SetPosition(_targetPosition);

            CallNextProcess();
        }

        /// <summary>
        /// 必要な参照をセットします。
        /// </summary>
        void SetUpReference()
        {
            if (_mapManager == null)
            {
                _mapManager = FindAnyObjectByType<MapManager>();
            }

            if (_playerMover == null)
            {
                _playerMover = FindAnyObjectByType<PlayerMover>();
            }
        }
    }
}