using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面のステータス表示のUIを制御するクラスです。
    /// </summary>
    public class MenuStatusUIController : MonoBehaviour, IMenuUIController
    {
        [Header("基本パラメータ")]
        /// <summary>
        /// キャラクターの名前テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _characterNameText;

        /// <summary>
        /// レベルの値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _levelValueText;

        /// <summary>
        /// HPの値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _hpValueText;

        /// <summary>
        /// MPの値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _mpValueText;

        /// <summary>
        /// 力の値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _strengthValueText;

        /// <summary>
        /// 身の守りの値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _guardValueText;

        /// <summary>
        /// 素早さの値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _speedValueText;

        /// <summary>
        /// 経験値の値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _currentExpValueText;

        /// <summary>
        /// 次のレベルまでの経験値の値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _nextExpValueText;

        [Header("ゴールド")]
        /// <summary>
        /// 所持金の値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _goldValueText;

        [Header("装備込みのパラメータ")]
        /// <summary>
        /// 武器の名前テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _weaponNameText;

        /// <summary>
        /// 防具の名前テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _armorNameText;

        /// <summary>
        /// 装備を含めた攻撃力の値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _equipmentAttackValueText;

        /// <summary>
        /// 装備を含めた防御力の値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _equipmentDefenseValueText;

        /// <summary>
        /// 装備を含めた素早さの値テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _equipmentSpeedValueText;

        /// <summary>
        /// キャラクターの名前をセットします。
        /// </summary>
        /// <param name="characterName">キャラクターの名前</param>
        public void SetCharacterNameText(string characterName)
        {
            _characterNameText.text = characterName;
        }

        /// <summary>
        /// レベルの値をセットします。
        /// </summary>
        /// <param name="level">レベル</param>
        public void SetLevelValueText(int level)
        {
            _levelValueText.text = level.ToString();
        }

        /// <summary>
        /// HPの値をセットします。
        /// </summary>
        /// <param name="currentHp">現在のHP</param>
        /// <param name="maxHp">最大HP</param>
        public void SetHpValueText(int currentHp, int maxHp)
        {
            _hpValueText.text = $"{currentHp} / {maxHp}";
        }

        /// <summary>
        /// MPの値をセットします。
        /// </summary>
        /// <param name="currentMp">現在のMP</param>
        /// <param name="maxMp">最大MP</param>
        public void SetMpValueText(int currentMp, int maxMp)
        {
            _mpValueText.text = $"{currentMp} / {maxMp}";
        }

        /// <summary>
        /// 力の値をセットします。
        /// </summary>
        /// <param name="strength">力の値</param>
        public void SetStrengthValueText(int strength)
        {
            _strengthValueText.text = strength.ToString();
        }

        /// <summary>
        /// 身の守りの値をセットします。
        /// </summary>
        /// <param name="guard">身の守りの値</param>
        public void SetGuardValueText(int guard)
        {
            _guardValueText.text = guard.ToString();
        }

        /// <summary>
        /// 素早さの値をセットします。
        /// </summary>
        /// <param name="speed">素早さの値</param>
        public void SetSpeedValueText(int speed)
        {
            _speedValueText.text = speed.ToString();
        }

        /// <summary>
        /// 現在の経験値をセットします。
        /// </summary>
        /// <param name="exp">現在の経験値</param>
        public void SetCurrentExpValueText(int exp)
        {
            _currentExpValueText.text = exp.ToString();
        }

        /// <summary>
        /// 次のレベルまでの経験値をセットします。
        /// </summary>
        /// <param name="exp">次のレベルまでの経験値</param>
        public void SetNextExpValueText(int exp)
        {
            _nextExpValueText.text = exp.ToString();
        }

        /// <summary>
        /// ゴールドの値をセットします。
        /// </summary>
        /// <param name="gold">ゴールドの値</param>
        public void SetGoldValueText(int gold)
        {
            _goldValueText.text = gold.ToString();
        }

        /// <summary>
        /// 武器の名前をセットします。
        /// </summary>
        /// <param name="weaponName">武器の名前</param>
        public void SetWeaponNameText(string weaponName)
        {
            _weaponNameText.text = weaponName;
        }

        /// <summary>
        /// 防具の名前をセットします。
        /// </summary>
        /// <param name="armorName">防具の名前</param>
        public void SetArmorNameText(string armorName)
        {
            _armorNameText.text = armorName;
        }

        /// <summary>
        /// 装備込みの攻撃力の値をセットします。
        /// </summary>
        /// <param name="attack">攻撃力の値</param>
        public void SetEquipmentAttackValueText(int attack)
        {
            _equipmentAttackValueText.text = attack.ToString();
        }

        /// <summary>
        /// 装備込みの防御力の値をセットします。
        /// </summary>
        /// <param name="defense">防御力の値</param>
        public void SetEquipmentDefenseValueText(int defense)
        {
            _equipmentDefenseValueText.text = defense.ToString();
        }

        /// <summary>
        /// 装備込みの素早さの値をセットします。
        /// </summary>
        /// <param name="speed">素早さの値</param>
        public void SetEquipmentSpeedValueText(int speed)
        {
            _equipmentSpeedValueText.text = speed.ToString();
        }

        /// <summary>
        /// テキストの表示を初期化します。
        /// </summary>
        public void InitializeText()
        {
            _characterNameText.text = string.Empty;
            _levelValueText.text = string.Empty;
            _hpValueText.text = string.Empty;
            _mpValueText.text = string.Empty;
            _strengthValueText.text = string.Empty;
            _guardValueText.text = string.Empty;
            _speedValueText.text = string.Empty;
            _currentExpValueText.text = string.Empty;
            _nextExpValueText.text = string.Empty;
            _goldValueText.text = string.Empty;
            _weaponNameText.text = string.Empty;
            _armorNameText.text = string.Empty;
            _equipmentAttackValueText.text = string.Empty;
            _equipmentDefenseValueText.text = string.Empty;
            _equipmentSpeedValueText.text = string.Empty;
        }

        /// <summary>
        /// UIを表示します。
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします。
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}