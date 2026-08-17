using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleRpg
{
    /// <summary>
    /// マップ情報の定義クラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/SimpleRpg/MapData")]
    public class MapData : ScriptableObject
    {
        /// <summary>
        /// マップIDと名前の対応リストです。
        /// </summary>
        public List<MapDataRecord> mapDataRecords;
    }
}