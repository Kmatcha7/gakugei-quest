using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// フラグの情報を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "FlagData", menuName = "Scriptable Objects/SimpleRpg/FlagData")]
    public class FlagNameData : ScriptableObject
    {
        /// <summary>
        /// フラグの名前一覧です。
        /// </summary>
        public List<string> flagNames;
    }
}