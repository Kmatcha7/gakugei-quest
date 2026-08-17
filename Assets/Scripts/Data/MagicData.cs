using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 魔法の情報を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "MagicData", menuName = "Scriptable Objects/SimpleRpg/MagicData")]
    public class MagicData : ScriptableObject
    {
        /// <summary>
        /// 魔法のIDです。
        /// </summary>
        public int magicId;

        /// <summary>
        /// 魔法の名前です。
        /// </summary>
        public string magicName;

        /// <summary>
        /// 魔法の説明です。
        /// </summary>
        public string magicDesc;

        /// <summary>
        /// 魔法の消費MPです。
        /// </summary>
        public int cost;

        /// <summary>
        /// 魔法の効果リストです。
        /// </summary>
        public List<MagicEffect> magicEffects;
        /// <summary>
        /// 魔法のアンロックフラグです。
        /// </summary>
        public string unlockFlagName;
    }
}