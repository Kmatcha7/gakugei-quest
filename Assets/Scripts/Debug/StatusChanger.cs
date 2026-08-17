using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 味方キャラクターのステータスを変更するデバッグ用クラスです。
    /// </summary>
    public class StatusChanger : MonoBehaviour
    {
        [Header("テスト用の設定")]
        /// <summary>
        /// 変更するHP量です。
        /// </summary>
        [SerializeField]
        int _hpDelta;

        /// <summary>
        /// 変更するMP量です。
        /// </summary>
        [SerializeField]
        int _mpDelta;

        [Header("ステータスを変更する")]
        /// <summary>
        /// ステータスを変更するフラグです。
        /// Inspectorウィンドウからチェックを入れると、ステータスを変更します。
        /// </summary>
        [SerializeField]
        bool _executeChange;

        void Update()
        {
            CheckChangeFlag();
        }

        /// <summary>
        /// ステータスを変更するフラグの状態をチェックします。
        /// </summary>
        void CheckChangeFlag()
        {
            // 定義データのロードを待つため、最初の5フレームは処理を抜けます。
            if (Time.frameCount < 5)
            {
                return;
            }

            if (!_executeChange)
            {
                return;
            }

            _executeChange = false;
            ChangeStatus();
        }

        /// <summary>
        /// ステータスを変動させます。
        /// </summary>
        void ChangeStatus()
        {
            int targetCharacterId = CharacterStatusManager.partyCharacter[0];
            CharacterStatusManager.ChangeCharacterStatus(targetCharacterId, _hpDelta, _mpDelta);
            SimpleLogger.Instance.Log($"キャラクターのステータスを変更しました。 ID: {targetCharacterId}, HP: {_hpDelta}, MP: {_mpDelta}");
        }
    }
}