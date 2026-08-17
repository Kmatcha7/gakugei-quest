using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターのステータスを管理するクラスです。
    /// </summary>
    public static class CharacterStatusManager
    {
        /// <summary>
        /// パーティ内にいるキャラクターのIDのリストです。
        /// </summary>
        public static List<int> partyCharacter;

        /// <summary>
        /// キャラクターのステータスのリストです。
        /// </summary>
        public static List<CharacterStatus> characterStatuses;

        /// <summary>
        /// プレイヤーの所持金です。
        /// </summary>
        public static int partyGold;

        /// <summary>
        /// プレイヤーの所持アイテムのリストです。
        /// </summary>
        public static List<PartyItemInfo> partyItemInfoList;

        /// <summary>
        /// アイテムを装備していない時のIDです。
        /// </summary>
        public static readonly int NoEquipmentId = 0;

        /// <summary>
        /// パーティ内のキャラクターのステータスをIDで取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static CharacterStatus GetCharacterStatusById(int characterId)
        {
            return characterStatuses.Find(character => character.characterId == characterId);
        }

        /// <summary>
        /// パーティ内のキャラクターの装備も含めたパラメータをIDで取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static BattleParameter GetCharacterBattleParameterById(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);

            BattleParameter baseParameter = new()
            {
                strength = parameterRecord.strength,
                guard = parameterRecord.guard,
                speed = parameterRecord.speed,
            };

            BattleParameter equipmentParameter = EquipmentCalculator.GetEquipmentParameter(characterStatus.equipWeaponId, characterStatus.equipArmorId);
            baseParameter.strength += equipmentParameter.strength;
            baseParameter.guard += equipmentParameter.guard;
            baseParameter.speed += equipmentParameter.speed;

            return baseParameter;
        }

        /// <summary>
        /// 対象のキャラクターのステータスを増減させます。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="hpDelta">増減させるHP</param>
        /// <param name="mpDelta">増減させるMP</param>
        public static void ChangeCharacterStatus(int characterId, int hpDelta, int mpDelta)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null)
            {
                Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {characterId}");
                return;
            }

            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);

            characterStatus.currentHp += hpDelta;
            if (characterStatus.currentHp > parameterRecord.hp)
            {
                characterStatus.currentHp = parameterRecord.hp;
            }
            else if (characterStatus.currentHp < 0)
            {
                characterStatus.currentHp = 0;
            }

            if (characterStatus.currentHp == 0)
            {
                characterStatus.isDefeated = true;
                return;
            }

            characterStatus.currentMp += mpDelta;
            if (characterStatus.currentMp > parameterRecord.mp)
            {
                characterStatus.currentMp = parameterRecord.mp;
            }
            else if (characterStatus.currentMp < 0)
            {
                characterStatus.currentMp = 0;
            }
        }

        /// <summary>
        /// パーティ内のキャラクターを全回復させます。
        /// </summary>
        public static void RefreshPartyCharacter()
        {
            foreach (int characterId in partyCharacter)
            {
                ChangeCharacterStatus(characterId, ValueSettings.MaxHp, ValueSettings.MaxMp);
                SetDefeatedFlag(characterId, false);
            }
        }

        /// <summary>
        /// 対象のキャラクターが倒れたかどうかを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="isDefeated">キャラクターが倒れたかどうかのフラグ</param>
        public static void SetDefeatedFlag(int characterId, bool isDefeated)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null)
            {
                Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {characterId}");
                return;
            }

            characterStatus.isDefeated = isDefeated;
        }

        /// <summary>
        /// 対象のキャラクターが倒れたかどうかを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static bool IsCharacterDefeated(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            return characterStatus.isDefeated;
        }

        /// <summary>
        /// 全てのキャラクターが倒れたかどうかを取得します。
        /// </summary>
        public static bool IsAllCharacterDefeated()
        {
            bool isAllDefeated = true;
            foreach (int characterId in partyCharacter)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (!characterStatus.isDefeated)
                {
                    isAllDefeated = false;
                    break;
                }
            }
            return isAllDefeated;
        }

        /// <summary>
        /// 引数のアイテムを使用します。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        public static void UseItem(int itemId)
        {
            var partyItemInfo = partyItemInfoList.Find(info => info.itemId == itemId);
            if (partyItemInfo == null)
            {
                Debug.LogWarning($"対象のアイテムを所持していません。 ID : {itemId}");
                return;
            }

            partyItemInfo.usedNum++;

            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (partyItemInfo.usedNum >= itemData.numberOfUse && itemData.numberOfUse > 0)
            {
                partyItemInfo.itemNum--;
            }

            if (partyItemInfo.itemNum <= 0)
            {
                partyItemInfoList.Remove(partyItemInfo);
            }
        }

        /// <summary>
        /// 引数のアイテムを指定した値だけ増加させます。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        /// <param name="itemNum">増加させる数</param>
        public static void IncreaseItem(int itemId, int itemNum)
        {
            // アイテムのIDが有効かどうか確認します。
            // ただし装備がない場合のIDに該当する場合はメッセージを出力しないようにします。
            if (!IsValidItem(itemId))
            {
                if (itemId != NoEquipmentId)
                {
                    SimpleLogger.Instance.LogWarning($"アイテムIDが無効です。 ID : {itemId}");
                }
                return;
            }

            // 増加用のメソッドで負の個数が指定された場合は、減少時のメソッドを呼び出します。
            if (itemNum <= 0)
            {
                int itemNumAbs = Mathf.Abs(itemNum);
                DecreaseItem(itemId, itemNumAbs);
                return;
            }

            var partyItemInfo = partyItemInfoList.Find(info => info.itemId == itemId);
            if (partyItemInfo == null)
            {
                // アイテムが存在しない場合、新たに追加します。
                partyItemInfo = new()
                {
                    itemId = itemId,
                    itemNum = itemNum
                };
                partyItemInfoList.Add(partyItemInfo);
            }
            else
            {
                partyItemInfo.itemNum += itemNum;
                if (partyItemInfo.itemNum > ValueSettings.MaxItemNum)
                {
                    partyItemInfo.itemNum = ValueSettings.MaxItemNum;
                }
            }
        }

        /// <summary>
        /// 引数のアイテムを指定した値だけ減少させます。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        /// <param name="itemNum">減少させる数</param>
        public static void DecreaseItem(int itemId, int itemNum)
        {
            // アイテムのIDが有効かどうか確認します。
            // ただし装備がない場合のIDに該当する場合はメッセージを出力しないようにします。
            if (!IsValidItem(itemId))
            {
                if (itemId != NoEquipmentId)
                {
                    SimpleLogger.Instance.LogWarning($"アイテムIDが無効です。 ID : {itemId}");
                }
                return;
            }

            // 減少用のメソッドで負の個数が指定された場合は、増加時のメソッドを呼び出します。
            if (itemNum < 0)
            {
                int itemNumAbs = Mathf.Abs(itemNum);
                IncreaseItem(itemId, itemNumAbs);
                return;
            }

            var partyItemInfo = partyItemInfoList.Find(info => info.itemId == itemId);
            if (partyItemInfo != null)
            {
                partyItemInfo.itemNum -= itemNum;
            }

            if (partyItemInfo.itemNum <= 0)
            {
                partyItemInfoList.Remove(partyItemInfo);
            }
        }

        /// <summary>
        /// 引数のアイテムIDが有効かどうかを確認します。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        static bool IsValidItem(int itemId)
        {
            // 装備がない場合のIDに該当する場合は無効とします。
            if (itemId == NoEquipmentId)
            {
                return false;
            }

            // アイテムのIDが定義されているか確認します。
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID : {itemId}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// HPが0でないパーティキャラクターの経験値を増加させます。
        /// </summary>
        /// <param name="exp">増加させる経験値</param>
        public static void IncreaseExp(int exp)
        {
            foreach (var characterId in partyCharacter)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (!characterStatus.isDefeated)
                {
                    characterStatus.exp += exp;
                }
            }
        }

        /// <summary>
        /// パーティの所持金を増加させます。
        /// </summary>
        /// <param name="gold">増加させる金額</param>
        public static void IncreaseGold(int gold)
        {
            partyGold += gold;
            if (partyGold < 0)
            {
                partyGold = 0;
            }
            else if (partyGold > ValueSettings.MaxGold)
            {
                partyGold = ValueSettings.MaxGold;
            }
        }

        /// <summary>
        /// 指定したキャラクターがレベルアップしたかどうかを返します。
        /// Trueでレベルアップしています。
        /// </summary>
        public static bool CheckLevelUp(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            var expTable = CharacterDataManager.GetExpTable();
            int targetLevel = 1;
            for (int i = 0; i < expTable.expRecords.Count; i++)
            {
                var expRecord = expTable.expRecords[i];
                if (characterStatus.exp >= expRecord.exp)
                {
                    targetLevel = expRecord.level;
                }
                else
                {
                    break;
                }
            }

            if (targetLevel > characterStatus.level)
            {
                characterStatus.level = targetLevel;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 現在のレベルで習得可能な魔法の内、まだ習得していないものを習得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static void LearnMagic(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (characterStatus == null)
            {
                Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {characterId}");
                return;
            }

            var learnableMagicList = CharacterDataManager.GetLearnableMagic(characterId, characterStatus.level);
            foreach (var magicData in learnableMagicList)
            {
                if (!characterStatus.magicList.Contains(magicData.magicId))
                {
                    characterStatus.magicList.Add(magicData.magicId);
                }
            }
        }
    }
}