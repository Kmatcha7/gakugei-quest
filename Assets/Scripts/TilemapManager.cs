using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleRpg
{
    /// <summary>
    /// Tilemapに関する機能を提供する管理クラスです。
    /// </summary>
    public class TilemapManager : MonoBehaviour
    {
        /// <summary>
        /// 地面用Tilemapへの参照です。
        /// </summary>
        [SerializeField]
        Tilemap _tilemapBase;

        /// <summary>
        /// 装飾品用Tilemapへの参照です。
        /// </summary>
        [SerializeField]
        Tilemap _tilemapProps;

        /// <summary>
        /// オーバーレイ用Tilemapへの参照です。
        /// </summary>
        [SerializeField]
        Tilemap _tilemapOverray;

        /// <summary>
        /// 侵入できないタイル一覧の定義ファイルです。
        /// </summary>
        [SerializeField]
        NoEntryTileData _noEntryTileData;

        /// <summary>
        /// イベント実行時にひとつ先まで確認するタイル一覧の定義ファイルです。
        /// </summary>
        [SerializeField]
        ThroughTileData _throughTileData;

        /// <summary>
        /// キャラクターがいるTilemap上の座標の辞書です。
        /// Key: キャラクターのインスタンスID
        /// Value: 座標
        /// </summary>
        [SerializeField]
        Dictionary<int, Vector3Int> _characterPositions = new();

        /// <summary>
        /// ワールド座標からTilemap上の論理的な座標を取得します。
        /// </summary>
        /// <param name="worldPos">変換元のワールド座標</param>
        public Vector3Int GetPositionOnTilemap(Vector3 worldPos)
        {
            var posOnTile = Vector3Int.zero;
            if (_tilemapBase != null)
            {
                posOnTile =_tilemapBase.WorldToCell(worldPos);
            }
            return posOnTile;
        }

        /// <summary>
        /// Tilemap上の座標からワールド座標を取得します。
        /// </summary>
        /// <param name="worldPos">変換元のTilemap上の座標</param>
        public Vector3 GetWorldPosition(Vector3Int posOnTile)
        {
            var worldPos = Vector3.zero;
            if (_tilemapBase != null)
            {
                worldPos = _tilemapBase.CellToWorld(posOnTile);
                worldPos += _tilemapBase.tileAnchor;
            }
            return worldPos;
        }

        /// <summary>
        /// 対象の座標に移動できるかどうかを確認します。
        /// </summary>
        /// <param name="targetPos">移動先のTilemap上の座標</param>
        public bool CanEntryTile(Vector3Int targetPos)
        {
            bool canEntry = true;

            if (_noEntryTileData == null)
            {
                SimpleLogger.Instance.LogWarning("侵入できないタイル一覧の定義ファイルがnullです。[NoEntryTileData]のフィールドに定義ファイルをアサインしてください。");
                return canEntry;
            }

            // 移動先の座標にキャラクターがいるか確認します。
            if (IsPositionUsed(targetPos))
            {
                canEntry = false;
                return canEntry;
            }

            // 各レイヤーのタイルを確認します。
            List<TileBase> targetTiles = GetTilesOnPosition(targetPos);

            // 定義ファイルのタイルと照合します。
            foreach (var tile in targetTiles)
            {
                if (tile == null)
                {
                    break;
                }

                if (_noEntryTileData.noEntryTiles == null)
                {
                    break;
                }

                var noEntryTile = _noEntryTileData.noEntryTiles.Find(t => t.name == tile.name);
                if (noEntryTile != null)
                {
                    canEntry = false;
                    break;
                }
            }

            return canEntry;
        }

        /// <summary>
        /// 対象の座標が、イベントの実行時にひとつ先まで確認する対象のタイルか確認します。
        /// </summary>
        /// <param name="targetPos">移動先のTilemap上の座標</param>
        public bool IsThroughTile(Vector3Int targetPos)
        {
            bool isThroughTile = false;

            if (_throughTileData == null)
            {
                SimpleLogger.Instance.LogWarning("ひとつ先まで確認するタイル一覧の定義ファイルがnullです。[ThroughTileData]のフィールドに定義ファイルをアサインしてください。");
                return isThroughTile;
            }

            // 各レイヤーのタイルを確認します。
            List<TileBase> targetTiles = GetTilesOnPosition(targetPos);

            // 定義ファイルのタイルと照合します。
            foreach (var tile in targetTiles)
            {
                if (tile == null)
                {
                    break;
                }

                if (_throughTileData.throughTiles == null)
                {
                    break;
                }

                var throughTile = _throughTileData.throughTiles.Find(t => t.name == tile.name);
                if (throughTile != null)
                {
                    isThroughTile = true;
                    break;
                }
            }

            return isThroughTile;
        }

        /// <summary>
        /// 各タイルマップにて指定した座標のタイルをリストとして取得します。
        /// </summary>
        /// <param name="targetPos">Tilemap上の座標</param>
        List<TileBase> GetTilesOnPosition(Vector3Int targetPos)
        {
            // 各レイヤーのタイルを確認します。
            List<TileBase> targetTiles = new();
            var baseTile = GetTileOnPos(_tilemapBase, targetPos);
            targetTiles.Add(baseTile);

            var propsTile = GetTileOnPos(_tilemapProps, targetPos);
            targetTiles.Add(propsTile);

            var overrayTile = GetTileOnPos(_tilemapOverray, targetPos);
            targetTiles.Add(overrayTile);
            return targetTiles;
        }

        /// <summary>
        /// 対象のTilemapにて、指定した位置のタイルを取得します。
        /// </summary>
        /// <param name="tilemap">対象のTilemap</param>
        /// <param name="targetPos">Tilemap上の座標</param>
        TileBase GetTileOnPos(Tilemap tilemap, Vector3Int targetPos)
        {
            TileBase tile = null;
            if (tilemap != null)
            {
                tile = tilemap.GetTile(targetPos);
            }
            return tile;
        }

        /// <summary>
        /// 他のキャラクターが移動対象の位置にいるか確認します。
        /// </summary>
        /// <param name="targetPos">Tilemap上の座標</param>
        bool IsPositionUsed(Vector3Int targetPos)
        {
            return _characterPositions.ContainsValue(targetPos);
        }

        /// <summary>
        /// 移動先の位置を辞書に登録します。
        /// </summary>
        /// <param name="instanceId">キャラクターのインスタンスID</param>
        /// <param name="targetPos">移動先のTilemap上の座標</param>
        public void ReservePosition(int instanceId, Vector3Int targetPos)
        {
            if (_characterPositions.ContainsKey(instanceId))
            {
                _characterPositions[instanceId] = targetPos;
            }
            else
            {
                _characterPositions.Add(instanceId, targetPos);
            }
        }

        /// <summary>
        /// タイルマップの各レイヤーをセットします。
        /// </summary>
        /// <param name="tilemapBase">地面用Tilemap</param>
        /// <param name="tilemapProps">装飾品用Tilemap</param>
        /// <param name="tilemapOverray">オーバーレイ用Tilemap</param>
        public void SetTilemaps(Tilemap tilemapBase, Tilemap tilemapProps, Tilemap tilemapOverray)
        {
            _tilemapBase = tilemapBase;
            _tilemapProps = tilemapProps;
            _tilemapOverray = tilemapOverray;
        }

        /// <summary>
        /// キャラクターの予約位置をクリアします。
        /// </summary>
        public void ResetPositions()
        {
            _characterPositions.Clear();
        }
    }
}