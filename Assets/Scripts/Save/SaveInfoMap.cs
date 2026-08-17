using System;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// セーブ時のマップ情報を定義するクラスです。
    /// </summary>
    [Serializable]
    public class SaveInfoMap
    {
        /// <summary>
        /// セーブ時にいるマップのIDです。
        /// </summary>
        public int mapId;

        /// <summary>
        /// セーブ時にいるマップの座標です。
        /// </summary>
        public Vector3Int mapPosition;
    }
}