using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleRpg
{
    /// <summary>
    /// イベント実行時にひとつ先まで確認するタイルの定義クラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "ThroughTileData", menuName = "Scriptable Objects/SimpleRpg/ThroughTileData")]
    public class ThroughTileData : ScriptableObject
    {
        /// <summary>
        /// イベント実行時にひとつ先まで確認するタイルの一覧です。
        /// </summary>
        public List<TileBase> throughTiles;
    }
}