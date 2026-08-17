using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘の結果処理を管理するクラスです。
    /// </summary>
    public class BattleResultManager : MonoBehaviour
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘中の敵キャラクターのデータを管理するクラスへの参照です。
        /// </summary>
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// メッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MessageWindowController _messageWindowController;

        /// <summary>
        /// メッセージをポーズするかどうかのフラグです。
        /// </summary>
        bool _pauseMessage;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラスへの参照</param>
        public void SetReferences(BattleManager battleManager)
        {
            _battleManager = battleManager;
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        }

        /// <summary>
        /// 戦闘に勝利した時の処理です。
        /// </summary>
        public void OnWin()
        {
            // 倒した敵の経験値とゴールドを集計します。
            int totalExp = 0;
            int gold = 0;
            foreach (var enemyStatus in _enemyStatusManager.GetEnemyStatusList())
            {
                totalExp += enemyStatus.enemyData.exp;
                gold += enemyStatus.enemyData.gold;
            }

            // プレイヤーの経験値とゴールドを更新します。
            CharacterStatusManager.IncreaseExp(totalExp);
            CharacterStatusManager.IncreaseGold(gold);

            StartCoroutine(WinMessageProcess(totalExp, gold));
        }

        /// <summary>
        /// 戦闘に勝利した時のメッセージ処理です。
        /// </summary>
        /// <param name="exp">獲得経験値</param>
        /// <param name="gold">獲得ゴールド</param>
        IEnumerator WinMessageProcess(int exp, int gold)
        {
            // 戦闘BGMを停止します。
            float fadeTime = 0.1f;
            AudioManager.Instance.StopAllBgm(fadeTime);

            // 戦闘勝利のジングルを再生します。
            AudioManager.Instance.PlaySe(SeNames.BattleWin);

            // パーティの最初のメンバーの名前を取得します。
            var firstMemberId = CharacterStatusManager.partyCharacter[0];
            var characterName = CharacterDataManager.GetCharacterName(firstMemberId);

            _pauseMessage = true;
            _messageWindowController.GenerateWinMessage(characterName);
            while (_pauseMessage)
            {
                yield return null;
            }

            if (exp > 0)
            {
                _pauseMessage = true;
                _messageWindowController.GenerateGetExpMessage(exp);
                while (_pauseMessage)
                {
                    yield return null;
                }
            }

            if (gold > 0)
            {
                _pauseMessage = true;
                _messageWindowController.GenerateGetGoldMessage(gold);
                while (_pauseMessage)
                {
                    yield return null;
                }
            }

            // キー入力を待ちます。
            _messageWindowController.StartKeyWait();
            while (_messageWindowController.IsWaitingKeyInput)
            {
                yield return null;
            }

            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);

            foreach (var id in CharacterStatusManager.partyCharacter)
            {
                var isLevelUp = CharacterStatusManager.CheckLevelUp(id);
                if (isLevelUp)
                {
                    // レベルアップ時のジングルを再生します。
                    AudioManager.Instance.PlaySe(SeNames.LevelUp);

                    _pauseMessage = true;
                    var characterStatus = CharacterStatusManager.GetCharacterStatusById(id);
                    var level = characterStatus.level;
                    CharacterStatusManager.LearnMagic(id);
                    characterName = CharacterDataManager.GetCharacterName(id);
                    _messageWindowController.GenerateLevelUpMessage(characterName, level);
                    while (_pauseMessage)
                    {
                        yield return null;
                    }

                    // キー入力を待ちます。
                    _messageWindowController.StartKeyWait();
                    while (_messageWindowController.IsWaitingKeyInput)
                    {
                        yield return null;
                    }

                    // 選択時のジングルを再生します。
                    AudioManager.Instance.PlaySe(SeNames.OK);
                }
            }

            // 処理の終了を通知します。
            _battleManager.OnFinishBattle();
        }

        /// <summary>
        /// 戦闘に敗北した時の処理です。
        /// </summary>
        public void OnLose()
        {
            StartCoroutine(LoseMessageProcess());
        }

        /// <summary>
        /// 戦闘に敗北した時のメッセージ処理です。
        /// </summary>
        IEnumerator LoseMessageProcess()
        {
            // 戦闘BGMを停止します。
            float fadeTime = 0.1f;
            AudioManager.Instance.StopAllBgm(fadeTime);

            // 戦闘敗北時のジングルを再生します。
            AudioManager.Instance.PlaySe(SeNames.Gameover);

            // パーティの最初のメンバーの名前を取得します。
            var firstMemberId = CharacterStatusManager.partyCharacter[0];
            var characterName = CharacterDataManager.GetCharacterName(firstMemberId);

            _pauseMessage = true;
            _messageWindowController.GenerateGameoverMessage(characterName);
            while (_pauseMessage)
            {
                yield return null;
            }

            float waitTime = 1.0f;
            yield return new WaitForSeconds(waitTime);

            // キー入力を待ちます。
            _messageWindowController.StartKeyWait();
            while (_messageWindowController.IsWaitingKeyInput)
            {
                yield return null;
            }

            // 処理の終了を通知します。
            _battleManager.OnFinishBattleWithGameover();
        }

        /// <summary>
        /// 次のメッセージを表示します。
        /// </summary>
        public void ShowNextMessage()
        {
            _pauseMessage = false;
        }
    }
}