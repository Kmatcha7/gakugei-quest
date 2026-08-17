using System;

namespace SimpleRpg
{
    /// <summary>
    /// 魔法の効果に関する設定を保持するクラスです。
    /// </summary>
    [Serializable]
    public class MagicEffect
    {
        /// <summary>
        /// 魔法のカテゴリです。
        /// </summary>
        public MagicCategory magicCategory;

        /// <summary>
        /// 魔法の効果範囲です。
        /// </summary>
        public EffectTarget effectTarget;

        /// <summary>
        /// 効果量です。
        /// </summary>
        public int value;
    }
}