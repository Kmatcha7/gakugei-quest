using System;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// セーブ時のファイル情報を定義するクラスです。
    /// </summary>
    [Serializable]
    public class SaveFile
    {
        /// <summary>
        /// セーブ枠のリストです。
        /// </summary>
        public List<SaveSlot> saveSlots;
    }
}