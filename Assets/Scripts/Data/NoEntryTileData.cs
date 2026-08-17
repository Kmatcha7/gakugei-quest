using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleRpg
{
    /// <summary>
    /// 侵入できないタイルの定義クラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "NoEntryTileData", menuName = "Scriptable Objects/SimpleRpg/NoEntryTileData")]
    public class NoEntryTileData : ScriptableObject
    {
        /// <summary>
        /// 侵入できないタイルの一覧です。
        /// </summary>
        public List<TileBase> noEntryTiles;
    }
}