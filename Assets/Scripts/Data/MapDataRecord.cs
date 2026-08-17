using System;

namespace SimpleRpg
{
    /// <summary>
    /// マップIDと名前を保持するクラスです。
    /// </summary>
    [Serializable]
    public class MapDataRecord
    {
        /// <summary>
        /// マップIDです。
        /// </summary>
        public int mapId;

        /// <summary>
        /// マップ名です。
        /// </summary>
        public string name;

        /// <summary>
        /// マップのBGM名です。
        /// </summary>
        public string bgmName;
    }
}