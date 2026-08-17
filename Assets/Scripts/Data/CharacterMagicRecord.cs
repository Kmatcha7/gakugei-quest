using System;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターが魔法を覚えるレベルを定義するクラスです。
    /// </summary>
    [Serializable]
    public class CharacterMagicRecord
    {
        /// <summary>
        /// 魔法を覚えるレベルの値です。
        /// </summary>
        public int level;

        /// <summary>
        /// 覚える魔法のIDです。
        /// </summary>
        public int magicId;
    }
}