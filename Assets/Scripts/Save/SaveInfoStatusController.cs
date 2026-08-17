using UnityEngine;
using System;

namespace SimpleRpg
{
    /// <summary>
    /// セーブデータとステータス情報をやりとりするクラスです。
    /// </summary>
    [Serializable]
    public class SaveInfoStatusController : MonoBehaviour
    {
        /// <summary>
        /// セーブデータ用のステータス情報を返します。
        /// </summary>
        public SaveInfoStatus GetSaveInfoStatus()
        {
            SaveInfoStatus saveInfoStatus = new()
            {
                partyCharacter = CharacterStatusManager.partyCharacter,
                characterStatuses = CharacterStatusManager.characterStatuses,
                partyGold = CharacterStatusManager.partyGold,
                partyItemInfoList = CharacterStatusManager.partyItemInfoList
            };
            return saveInfoStatus;
        }

        /// <summary>
        /// セーブデータから読み取ったステータス情報をセットします。
        /// </summary>
        /// <param name="saveInfoStatus">ステータス情報</param>
        public void SetSaveInfoStatus(SaveInfoStatus saveInfoStatus)
        {
            CharacterStatusManager.partyCharacter = saveInfoStatus.partyCharacter;
            CharacterStatusManager.characterStatuses = saveInfoStatus.characterStatuses;
            CharacterStatusManager.partyGold = saveInfoStatus.partyGold;
            CharacterStatusManager.partyItemInfoList = saveInfoStatus.partyItemInfoList;
        }
    }
}