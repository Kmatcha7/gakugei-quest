using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// メニューの装備画面で情報表示のUIを制御するクラスです。
    /// </summary>
    public class MenuEquipmentInformationUIController : MonoBehaviour, IMenuUIController
    {
        /// <summary>
        /// 装備の説明テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _descriptionText;

        /// <summary>
        /// キャラクター名のテキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _characterNameText;

        /// <summary>
        /// 攻撃力のテキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _atkValueText;

        /// <summary>
        /// 攻撃力の変動分テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _atkDeltaValueText;

        /// <summary>
        /// 防御力のテキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _defValueText;

        /// <summary>
        /// 防御力の変動分テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _defDeltaValueText;

        /// <summary>
        /// 素早さのテキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _speedValueText;

        /// <summary>
        /// 素早さの変動分テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _speedDeltaValueText;

        /// <summary>
        /// 値が変わらないときの色です。
        /// </summary>
        [SerializeField]
        Color _keepValueColor = Color.white;

        /// <summary>
        /// 値が増えるときの色です。
        /// </summary>
        [SerializeField]
        Color _upValueColor = Color.cyan;

        /// <summary>
        /// 値が減るときの色です。
        /// </summary>
        [SerializeField]
        Color _downValueColor = Color.gray;

        /// <summary>
        /// アイテムの説明テキストをセットします。
        /// </summary>
        /// <param name="description">説明</param>
        public void SetDescription(string description)
        {
            _descriptionText.text = description;
        }

        /// <summary>
        /// キャラクターの名前テキストをセットします。
        /// </summary>
        /// <param name="name">キャラクターの名前</param>
        public void SetCharacterName(string name)
        {
            _characterNameText.text = name;
        }

        /// <summary>
        /// テキストの色をセットします。
        /// </summary>
        /// <param name="targetText">対象のテキスト</param>
        /// <param name="deltaValue">値の変動量</param>
        void SetTextColor(TextMeshProUGUI targetText, int deltaValue)
        {
            Color color = _keepValueColor;
            if (deltaValue > 0)
            {
                color = _upValueColor;
            }
            else if (deltaValue < 0)
            {
                color = _downValueColor;
            }
            targetText.color = color;
        }

        /// <summary>
        /// 攻撃力の値をセットします。
        /// </summary>
        /// <param name="atkValue">攻撃力</param>
        /// <param name="atkDeltaValue">攻撃力の変動分</param>
        public void SetAtkText(int atkValue, int atkDeltaValue)
        {
            _atkValueText.text = atkValue.ToString();
            _atkDeltaValueText.text = atkDeltaValue.ToString();
            if (atkDeltaValue >= 0)
            {
                _atkDeltaValueText.text = "+" + atkDeltaValue.ToString();
            }
            SetTextColor(_atkDeltaValueText, atkDeltaValue);
        }

        /// <summary>
        /// 防御力の値をセットします。
        /// </summary>
        /// <param name="defValue">防御力</param>
        /// <param name="defDeltaValue">防御力の変動分</param>
        public void SetDefText(int defValue, int defDeltaValue)
        {
            _defValueText.text = defValue.ToString();
            _defDeltaValueText.text = defDeltaValue.ToString();
            if (defDeltaValue >= 0)
            {
                _defDeltaValueText.text = "+" + defDeltaValue.ToString();
            }
            SetTextColor(_defDeltaValueText, defDeltaValue);
        }

        /// <summary>
        /// 素早さの値をセットします。
        /// </summary>
        /// <param name="speedValue">素早さ</param>
        /// <param name="speedDeltaValue">素早さの変動分</param>
        public void SetSpeedText(int speedValue, int speedDeltaValue)
        {
            _speedValueText.text = speedValue.ToString();
            _speedDeltaValueText.text = speedDeltaValue.ToString();
            if (speedDeltaValue >= 0)
            {
                _speedDeltaValueText.text = "+" + speedDeltaValue.ToString();
            }
            SetTextColor(_speedDeltaValueText, speedDeltaValue);
        }

        /// <summary>
        /// 増減分テキストの値をクリアします。
        /// </summary>
        public void ClearDeltaValues()
        {
            _atkDeltaValueText.text = "";
            _defDeltaValueText.text = "";
            _speedDeltaValueText.text = "";
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