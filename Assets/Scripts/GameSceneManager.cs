using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// Gameシーンの動作を管理するクラスです。
    /// </summary>
    public class GameSceneManager : MonoBehaviour, IMapLoadCallback
    {
        /// <summary>
        /// マップ機能を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MapManager _mapManager;

        /// <summary>
        /// セーブデータの管理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveDataManager _saveDataManager;

        void Start()
        {
            _mapManager.LoadMapPrefab(this);
        }

        /// <summary>
        /// ロードが完了した時に呼ばれるコールバックです。
        /// </summary>
        public void OnFinishedLoad()
        {
            ReadyData();
        }

        /// <summary>
        /// はじめからまたはつづきからに応じて、ゲーム開始時に必要なデータを準備します。
        /// </summary>
        void ReadyData()
        {
            // このシーンから開始した場合は初期化処理を行います。
            if (!GameStartInfoHolder.isThroughInitScene)
            {
                StartCoroutine(SetUpProcess());
            }
            else
            {
                StartGame();
            }
        }

        /// <summary>
        /// ゲームの開始処理を行います。
        /// </summary>
        void StartGame()
        {
            if (GameStartInfoHolder.isNewGame)
            {
                NewGame();
            }
            else
            {
                ContinueGame();
            }
        }

        /// <summary>
        /// はじめからを選択した場合の処理です。
        /// </summary>
        void NewGame()
        {
            _mapManager.ShowStartMap();
            FadeManager.Instance.SetCallback(null);
            FadeManager.Instance.FadeInScreen();
        }

        /// <summary>
        /// つづきからを選択した場合の処理です。
        /// </summary>
        void ContinueGame()
        {
            // タイトル画面で選択したセーブ枠のデータをロードします。
            _saveDataManager.SetLoadedData(GameStartInfoHolder.loadedSlotId);
            FadeManager.Instance.SetCallback(null);
            FadeManager.Instance.FadeInScreen();
        }

        /// <summary>
        /// このシーンから開始した場合の初期化処理を行います。
        /// </summary>
        IEnumerator SetUpProcess()
        {
            ResourceLoader.LoadDefinitionData();
            yield return new WaitForSeconds(0.5f);

            var characterStatusSetter = FindAnyObjectByType<CharacterStatusSetter>();
            if (characterStatusSetter != null)
            {
                characterStatusSetter.SetUpCharacterStatus();
            }

            GameStartInfoHolder.isThroughInitScene = true;
            ReadyData();
        }
    }
}