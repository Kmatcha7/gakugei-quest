using System;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// セーブ時のフラグ情報を定義するクラスです。
    /// </summary>
    [Serializable]
    public class SaveInfoFlag
    {
        /// <summary>
        /// フラグの状態を保持するリストです。
        /// </summary>
        public List<FlagState> flagStates;
    }
}