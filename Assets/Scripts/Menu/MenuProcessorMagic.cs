using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// メニューウィンドウで魔法の使用処理を制御するクラスです。
    /// </summary>
    public class MenuProcessorMagic : MonoBehaviour
    {
        /// <summary>
        /// メニュー画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuItemWindowController _windowController;

        /// <summary>
        /// メニューウィンドウにて魔法に関する処理を制御するクラスへの参照です。
        /// </summary>
        MenuItemWindowMagicController _magicController;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MapMessageWindowController _mapMessageWindowController;

        /// <summary>
        /// 魔法効果をポーズするかどうかのフラグです。
        /// </summary>
        bool _pauseMagicEffect;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(MenuItemWindowController windowController)
        {
            _windowController = windowController;
            _magicController = windowController.GetMagicController();
            _mapMessageWindowController = windowController.GetMenuManager().GetMessageWindowController();
        }

        /// <summary>
        /// 魔法のアクションを処理します。
        /// </summary>
        public void UseSelectedMagic(int magicId)
        {
            var magicData = MagicDataManager.GetMagicDataById(magicId);
            if (magicData == null)
            {
                SimpleLogger.Instance.LogWarning($"魔法データが見つかりませんでした。 ID: {magicId}");
                return;
            }

            bool canSelect = _magicController.CanSelectMagic(magicData);
            if (!canSelect)
            {
                return;
            }

            // 消費MPの分だけMPを減らします。
            int hpDelta = 0;
            int mpDelta = magicData.cost * -1;
            int targetCharacterId = CharacterStatusManager.partyCharacter[0];
            CharacterStatusManager.ChangeCharacterStatus(targetCharacterId, hpDelta, mpDelta);
            StartCoroutine(ProcessMagicActionCoroutine(magicData));
        }

        /// <summary>
        /// 魔法のアクションを処理するコルーチンです。
        /// </summary>
        IEnumerator ProcessMagicActionCoroutine(MagicData magicData)
        {
            // 魔法の詠唱時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Magic);

            // 魔法の効果を処理します。
            foreach (var magicEffect in magicData.magicEffects)
            {
                if (magicEffect.magicCategory == MagicCategory.Recovery)
                {
                    int hpDelta = BattleCalculator.CalculateHealValue(magicEffect.value);
                    int mpDelta = 0;
                    int targetCharacterId = CharacterStatusManager.partyCharacter[0];
                    CharacterStatusManager.ChangeCharacterStatus(targetCharacterId, hpDelta, mpDelta);

                    _pauseMagicEffect = true;
                    StartCoroutine(ShowMagicHealMessage(targetCharacterId, magicData.magicName, hpDelta));
                }
                else
                {
                    Debug.LogWarning($"未定義の魔法効果です。 ID: {magicData.magicId}");
                }

                while (_pauseMagicEffect)
                {
                    yield return null;
                }
            }

            _magicController.OnFinishedMagicProcess();
            _mapMessageWindowController.HideWindow();
        }

        /// <summary>
        /// 回復魔法のメッセージを表示します。
        /// </summary>
        IEnumerator ShowMagicHealMessage(int characterId, string magicName, int healValue)
        {
            var characterName = CharacterDataManager.GetCharacterName(characterId);
            string actorName = characterName;
            string targetName = characterName;

            _mapMessageWindowController.SetUpController(_windowController);
            _mapMessageWindowController.HidePager();
            _mapMessageWindowController.ShowWindow();

            _windowController.SetCanSelectState(false);
            _windowController.SetPauseMessageState(true);
            _mapMessageWindowController.GenerateMagicCastMessage(actorName, magicName);
            while (_windowController.IsPausedMessage)
            {
                yield return null;
            }

            // 回復の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Heal);

            _windowController.SetPauseMessageState(true);
            _mapMessageWindowController.GenerateHpHealMessage(targetName, healValue);
            _windowController.UpdateStatus();
            while (_windowController.IsPausedMessage)
            {
                yield return null;
            }

            // キー入力を待ちます。
            _mapMessageWindowController.StartKeyWait();
            while (_mapMessageWindowController.IsWaitingKeyInput)
            {
                yield return null;
            }

            _pauseMagicEffect = false;
        }
    }
}