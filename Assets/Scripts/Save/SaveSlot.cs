using System;

namespace SimpleRpg
{
    /// <summary>
    /// セーブ時のセーブ枠の情報を定義するクラスです。
    /// </summary>
    [Serializable]
    public class SaveSlot
    {
        /// <summary>
        /// セーブ枠のIDです。
        /// </summary>
        public int slotId;

        /// <summary>
        /// セーブ時のキャラクターのステータス情報を定義するクラスへの参照です。
        /// </summary>
        public SaveInfoStatus saveInfoStatus;

        /// <summary>
        /// セーブ時のマップ情報を定義するクラスへの参照です。
        /// </summary>
        public SaveInfoMap saveInfoMap;

        /// <summary>
        /// セーブ時のフラグ情報を定義するクラスへの参照です。
        /// </summary>
        public SaveInfoFlag saveInfoFlag;
    }
}