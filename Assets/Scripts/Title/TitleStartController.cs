using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面のはじめからのメニューを制御するクラスです。
    /// </summary>
    public class TitleStartController : MonoBehaviour, IFadeCallback
    {
        /// <summary>
        /// 味方キャラクターのステータスをセットアップするクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterStatusSetter _characterStatusSetter;

        /// <summary>
        /// ゲーム用のシーン名です。
        /// </summary>
        readonly string GameSceneName = "Game";

        /// <summary>
        /// 最初からゲームを開始する準備を行います。
        /// </summary>
        public void StartNewGame()
        {
            SimpleLogger.Instance.Log("はじめからゲームを開始します。");
            GameStartInfoHolder.isNewGame = true;

            FadeManager.Instance.SetCallback(this);
            FadeManager.Instance.FadeOutScreen();
        }

        /// <summary>
        /// 続きからゲームを開始する準備を行います。
        /// </summary>
        /// <param name="slotId">セーブ枠のID</param>
        public void ContinueGame(int slotId)
        {
            SimpleLogger.Instance.Log("つづきからゲームを開始します。");
            GameStartInfoHolder.isNewGame = false;
            GameStartInfoHolder.loadedSlotId = slotId;

            FadeManager.Instance.SetCallback(this);
            FadeManager.Instance.FadeOutScreen();
        }

        /// <summary>
        /// フェードが完了した時に呼ばれるコールバックです。
        /// </summary>
        public void OnFinishedFade()
        {
            StartCoroutine(SwitchSceneProcess());
        }

        /// <summary>
        /// シーンを切り替える処理を行います。
        /// </summary>
        IEnumerator SwitchSceneProcess()
        {
            // BGMの停止を待ちます。
            AudioManager.Instance.StopAllBgm();
            yield return new WaitForSeconds(0.25f);

            _characterStatusSetter.SetUpCharacterStatus();
            FlagManager.Instance.InitializeFlagList();

            // シーンをロードします。
            SceneManager.LoadScene(GameSceneName);
        }
    }
}