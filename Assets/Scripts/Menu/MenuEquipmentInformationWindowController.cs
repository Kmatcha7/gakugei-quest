using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニューの装備画面で情報表示のウィンドウを制御するクラスです。
    /// </summary>
    public class MenuEquipmentInformationWindowController : MonoBehaviour, IMenuWindowController
    {
        /// <summary>
        /// メニューの装備画面で情報表示のUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuEquipmentInformationUIController _uiController;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(MenuManager menuManager)
        {
            
        }

        /// <summary>
        /// アイテムの説明テキストをセットします。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        public void SetDescription(int itemId)
        {
            string description = string.Empty;
            ItemData itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData != null)
            {
                description = itemData.itemDesc;
            }
            _uiController.SetDescription(description);
        }

        /// <summary>
        /// キャラクターの名前テキストをセットします。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public void SetCharacterName(int characterId)
        {
            string characterName = CharacterDataManager.GetCharacterName(characterId);
            _uiController.SetCharacterName(characterName);
        }

        /// <summary>
        /// 現在のステータスをセットします。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public void SetStatusValue(int characterId)
        {
            int atkValue = 0;
            int guardValue = 0;
            int speedValue = 0;

            var battleParameter = CharacterStatusManager.GetCharacterBattleParameterById(characterId);
            if (battleParameter != null)
            {
                atkValue = battleParameter.strength;
                guardValue = battleParameter.guard;
                speedValue = battleParameter.speed;
            }
            else
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
            }

            _uiController.SetAtkText(atkValue, 0);
            _uiController.SetDefText(guardValue, 0);
            _uiController.SetSpeedText(speedValue, 0);
            _uiController.ClearDeltaValues();
        }

        /// <summary>
        /// 選択したアイテムによる増減分を表示します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="newWeaponId">新しい武器のID</param>
        /// <param name="newArmorId">新しい防具のID</param>
        public void SetSelectedItemStatusValue(int characterId, int newWeaponId, int newArmorId)
        {
            var currentBattleParameter = CharacterStatusManager.GetCharacterBattleParameterById(characterId);
            if (currentBattleParameter == null)
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
                _uiController.ClearDeltaValues();
                return;
            }

            var newParameter = GetNewBattleParameterById(characterId, newWeaponId, newArmorId);
            if (newParameter == null)
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
                _uiController.ClearDeltaValues();
                return;
            }

            int atkDeltaValue = newParameter.strength - currentBattleParameter.strength;
            int guardDeltaValue = newParameter.guard - currentBattleParameter.guard;
            int speedDeltaValue = newParameter.speed - currentBattleParameter.speed;

            _uiController.SetAtkText(newParameter.strength, atkDeltaValue);
            _uiController.SetDefText(newParameter.guard, guardDeltaValue);
            _uiController.SetSpeedText(newParameter.speed, speedDeltaValue);
        }

        /// <summary>
        /// パーティ内のキャラクターの装備も含めたパラメータをIDで取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        /// <param name="newWeaponId">新しい武器のID</param>
        /// <param name="newArmorId">新しい防具のID</param>
        public BattleParameter GetNewBattleParameterById(int characterId, int newWeaponId, int newArmorId)
        {
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            if (characterStatus == null || parameterTable == null)
            {
                SimpleLogger.Instance.LogError($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
                return null;
            }

            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);

            BattleParameter baseParameter = new()
            {
                strength = parameterRecord.strength,
                guard = parameterRecord.guard,
                speed = parameterRecord.speed,
            };

            BattleParameter equipmentParameter = EquipmentCalculator.GetEquipmentParameter(newWeaponId, newArmorId);
            baseParameter.strength += equipmentParameter.strength;
            baseParameter.guard += equipmentParameter.guard;
            baseParameter.speed += equipmentParameter.speed;

            return baseParameter;
        }

        /// <summary>
        /// 装備箇所ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// 装備箇所ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}