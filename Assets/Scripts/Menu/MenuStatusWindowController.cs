using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面のステータス表示のウィンドウを制御するクラスです。
    /// </summary>
    public class MenuStatusWindowController : MonoBehaviour, IMenuWindowController
    {
        /// <summary>
        /// メニュー画面に関する機能を管理するクラスへの参照です。
        /// </summary>
        MenuManager _menuManager;

        /// <summary>
        /// メニュー画面のステータス表示のUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuStatusUIController _uiController;

        /// <summary>
        /// ステータス画面を閉じられるかどうかのフラグです。
        /// </summary>
        bool _canClose;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        /// <summary>
        /// キャラクターのステータスを画面に表示します。
        /// </summary>
        public void SetUpStatus()
        {
            int characterId = CharacterStatusManager.partyCharacter[0];

            CharacterStatus characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (characterStatus == null)
            {
                SimpleLogger.Instance.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID: {characterId}");
                return;
            }

            ParameterTable parameterTable = CharacterDataManager.GetParameterTable(characterId);
            if (parameterTable == null)
            {
                SimpleLogger.Instance.LogWarning($"キャラクターのパラメータが見つかりませんでした。 ID: {characterId}");
                return;
            }

            ParameterRecord parameterRecord = parameterTable.parameterRecords.Find(x => x.level == characterStatus.level);
            if (parameterRecord == null)
            {
                SimpleLogger.Instance.LogWarning($"レベルに対応するパラメータが見つかりませんでした。 ID: {characterId}, レベル: {characterStatus.level}");
                return;
            }

            // 基本パラメータをセットします。
            string characterName = CharacterDataManager.GetCharacterName(characterId);
            _uiController.SetCharacterNameText(characterName);
            _uiController.SetLevelValueText(characterStatus.level);
            _uiController.SetHpValueText(characterStatus.currentHp, parameterRecord.hp);
            _uiController.SetMpValueText(characterStatus.currentMp, parameterRecord.mp);
            _uiController.SetStrengthValueText(parameterRecord.strength);
            _uiController.SetGuardValueText(parameterRecord.guard);
            _uiController.SetSpeedValueText(parameterRecord.speed);
            _uiController.SetCurrentExpValueText(characterStatus.exp);

            int nextExp = GetNextExp(characterStatus);
            _uiController.SetNextExpValueText(nextExp);

            // ゴールドをセットします。
            _uiController.SetGoldValueText(CharacterStatusManager.partyGold);

            // 装備込みのパラメータをセットします。
            var weaponData = ItemDataManager.GetItemDataById(characterStatus.equipWeaponId);
            string weaponName = "なし";
            if (weaponData != null)
            {
                weaponName = weaponData.itemName;
            }

            var armorData = ItemDataManager.GetItemDataById(characterStatus.equipArmorId);
            string armorName = "なし";
            if (armorData != null)
            {
                armorName = armorData.itemName;
            }

            var parameter = CharacterStatusManager.GetCharacterBattleParameterById(characterId);
            _uiController.SetWeaponNameText(weaponName);
            _uiController.SetArmorNameText(armorName);
            _uiController.SetEquipmentAttackValueText(parameter.strength);
            _uiController.SetEquipmentDefenseValueText(parameter.guard);
            _uiController.SetEquipmentSpeedValueText(parameter.speed);
        }

        /// <summary>
        /// 次のレベルまでに必要な経験値を取得します。
        /// </summary>
        int GetNextExp(CharacterStatus status)
        {
            int nextExp = 0;
            var expTable = CharacterDataManager.GetExpTable();
            if (expTable == null)
            {
                return nextExp;
            }

            int nextLevel = status.level + 1;
            var expRecord = expTable.expRecords.Find(x => x.level == nextLevel);
            if (expRecord != null)
            {
                nextExp = expRecord.exp - status.exp;
                nextExp = Mathf.Max(nextExp, 0);
            }
            return nextExp;
        }

        void Update()
        {
            CheckKeyInput();
        }

        /// <summary>
        /// キー入力を検知します。
        /// </summary>
        void CheckKeyInput()
        {
            if (_menuManager == null)
            {
                return;
            }

            if (_menuManager.MenuPhase != MenuPhase.Status)
            {
                return;
            }

            if (!_canClose)
            {
                return;
            }

            // ステータス画面では、決定ボタンとキャンセルボタンの入力を検知します。
            if (InputGameKey.ConfirmButton())
            {
                OnPressedConfirmButton();
            }
            else if (InputGameKey.CancelButton())
            {
                OnPressedCancelButton();
            }
        }

        /// <summary>
        /// 決定ボタンが押された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);

            StartCoroutine(HideProcess());
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        void OnPressedCancelButton()
        {
            // キャンセル時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Cancel);

            StartCoroutine(HideProcess());
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        IEnumerator HideProcess()
        {
            _canClose = false;
            yield return null;

            _menuManager.OnStatusCanceled();
            HideWindow();
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.InitializeText();
            SetUpStatus();
            _uiController.Show();
            _canClose = false;

            // ステータス画面を開いたフレームでボタンの検知をしないようにします。
            StartCoroutine(SetCloseStateDelay());
        }

        /// <summary>
        /// ステータス画面を閉じられるフラグをセットします。
        /// </summary>
        IEnumerator SetCloseStateDelay()
        {
            yield return null;
            _canClose = true;
        }

        /// <summary>
        /// 選択ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}