using System;

namespace SimpleRpg
{
    /// <summary>
    /// フラグの状態を保持するクラスです。
    /// </summary>
    [Serializable]
    public class FlagState
    {
        /// <summary>
        /// フラグの名前です。
        /// </summary>
        public string flagName;

        /// <summary>
        /// フラグの状態を表す値です。
        /// </summary>
        public bool state;
    }
}