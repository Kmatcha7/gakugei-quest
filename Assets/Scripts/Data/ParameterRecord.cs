using System;

namespace SimpleRpg
{
    /// <summary>
    /// レベルに対応するパラメータを保持するクラスです。
    /// </summary>
    [Serializable]
    public class ParameterRecord
    {
        /// <summary>
        /// レベルの値です。
        /// </summary>
        public int level;

        /// <summary>
        /// HPの値です。
        /// </summary>
        public int hp;

        /// <summary>
        /// MPの値です。
        /// </summary>
        public int mp;

        /// <summary>
        /// 力の値です。
        /// </summary>
        public int strength;

        /// <summary>
        /// 身のまもりの値です。
        /// </summary>
        public int guard;

        /// <summary>
        /// 素早さの値です。
        /// </summary>
        public int speed;
    }
}