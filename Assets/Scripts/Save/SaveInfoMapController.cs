using UnityEngine;
using System;

namespace SimpleRpg
{
    /// <summary>
    /// セーブデータとマップ情報をやりとりするクラスです。
    /// </summary>
    [Serializable]
    public class SaveInfoMapController : MonoBehaviour
    {
        /// <summary>
        /// マップ機能を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MapManager _mapManager;

        /// <summary>
        /// 操作キャラの移動制御を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        PlayerMover _playerMover;

        /// <summary>
        /// セーブデータ用のマップ情報を返します。
        /// </summary>
        public SaveInfoMap GetSaveInfoMap()
        {
            // マップIDを取得します。
            int mapId = 0;
            var mapController = _mapManager.GetCurrentMapController();
            if (mapController == null)
            {
                SimpleLogger.Instance.LogWarning("MapControllerが取得できませんでした。");
            }
            else
            {
                mapId = mapController.MapId;
            }

            // マップの位置を取得します。
            var position = Vector3Int.zero;
            if (_playerMover == null)
            {
                SimpleLogger.Instance.LogWarning("PlayerMoverへの参照がnullです。");
            }
            else
            {
                position = _playerMover.PosOnTile;
            }

            SaveInfoMap saveInfoMap = new()
            {
                mapId = mapId,
                mapPosition = position
            };
            return saveInfoMap;
        }

        /// <summary>
        /// セーブデータから読み取ったマップ情報をセットします。
        /// </summary>
        /// <param name="saveInfoMap">マップ情報</param>
        public void SetSaveInfoMap(SaveInfoMap saveInfoMap)
        {
            _mapManager.ShowMap(saveInfoMap.mapId);
            _playerMover.SetPosition(saveInfoMap.mapPosition);
        }
    }
}