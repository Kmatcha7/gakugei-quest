using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 味方キャラクターのステータスをセットアップするクラスです。
    /// </summary>
    public class CharacterStatusSetter : MonoBehaviour
    {
        /// <summary>
        /// 味方キャラクターのステータスをセットします。
        /// </summary>
        [SerializeField]
        List<int> _partyCharacters = new() {1};

        /// <summary>
        /// 味方キャラクターのレベルです。
        /// </summary>
        [SerializeField]
        int _playerLevel = 1;

        /// <summary>
        /// 装備中の武器のIDです。
        /// </summary>
        [SerializeField]
        int _weaponId;

        /// <summary>
        /// 装備中の防具のIDです。
        /// </summary>
        [SerializeField]
        int _armorId;

        /// <summary>
        /// アイテム所持数の設定です。
        /// </summary>
        [SerializeField]
        List<PartyItemInfo> _partyItemInfoList = new();

        /// <summary>
        /// 味方キャラクターのステータスをセットします。
        /// </summary>
        public void SetUpCharacterStatus()
        {
            SetPlayerStatus();
            SetPartyItems();
        }

        /// <summary>
        /// 味方キャラクターのステータスをセットします。
        /// </summary>
        void SetPlayerStatus()
        {
            var level = _playerLevel;
            var exp = GetExp(level);
            
            List<CharacterStatus> characterStatuses = new();

            foreach (int characterId in _partyCharacters)
            {
                // レベルに対応するパラメータデータを取得します。
                var parameterTable = CharacterDataManager.GetParameterTable(characterId);
                var parameterRecord = parameterTable.parameterRecords.Find(record => record.level == level);

                // 指定したレベルまでに覚えている魔法のIDをリスト化します。
                var magicList = GetMagicIdList(characterId, level);

                // キャラクターのステータスを設定します。
                CharacterStatus status = new()
                {
                    characterId = characterId,
                    level = level,
                    exp = exp,
                    currentHp = parameterRecord.hp,
                    currentMp = parameterRecord.mp,
                    equipWeaponId = _weaponId,
                    equipArmorId = _armorId,
                    magicList = magicList,
                };

                characterStatuses.Add(status);
            }

            CharacterStatusManager.characterStatuses = characterStatuses;

            // パーティにいるキャラクターのIDをセットします。
            CharacterStatusManager.partyCharacter = _partyCharacters;

            // 所持アイテムをセットします。
            CharacterStatusManager.partyItemInfoList = new();

            // 所持金をセットします。
            CharacterStatusManager.partyGold = 0;
        }

        /// <summary>
        /// キャラクターが覚えている魔法のIDリストを返します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <param name="level">キャラクターのレベル</param>
     List<int> GetMagicIdList(int characterId, int level)
    {
        var unlockedMagic = MagicDataManager.GetUnlockedMagicList();

        List<int> magicList = new();

        foreach (var magic in unlockedMagic)
        {
            magicList.Add(magic.magicId);
        }

        return magicList;
    }

        /// <summary>
        /// パーティの所持アイテムをセットします。
        /// </summary>
        void SetPartyItems()
        {
            CharacterStatusManager.partyItemInfoList = _partyItemInfoList;
        }

        public void UpdateMagicOnly()
        {
            foreach (var status in CharacterStatusManager.characterStatuses)
            {
                var unlockedMagic = MagicDataManager.GetUnlockedMagicList();

                List<int> magicList = new();

                foreach (var magic in unlockedMagic)
                {
                    magicList.Add(magic.magicId);
                }

                status.magicList = magicList;
            }
        }

        /// <summary>
        /// レベルに応じた経験値の値を取得します。
        /// </summary>
        int GetExp(int level)
        {
            int exp = 0;
            var expTable = CharacterDataManager.GetExpTable();
            if (expTable == null)
            {
                return exp;
            }

            var expRecord = expTable.expRecords.Find(record => record.level == level);
            if (expRecord != null)
            {
                exp = expRecord.exp;
            }

            return exp;
        }
    }
}