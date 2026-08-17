using System.Collections.Generic;
using System;

namespace SimpleRpg
{
    /// <summary>
    /// セーブ時のキャラクターのステータス情報を定義するクラスです。
    /// </summary>
    [Serializable]
    public class SaveInfoStatus
    {
        /// <summary>
        /// パーティ内にいるキャラクターのIDのリストです。
        /// </summary>
        public List<int> partyCharacter;

        /// <summary>
        /// キャラクターのステータスのリストです。
        /// </summary>
        public List<CharacterStatus> characterStatuses;

        /// <summary>
        /// プレイヤーの所持金です。
        /// </summary>
        public int partyGold;

        /// <summary>
        /// プレイヤーの所持アイテムのリストです。
        /// </summary>
        public List<PartyItemInfo> partyItemInfoList;
    }
}