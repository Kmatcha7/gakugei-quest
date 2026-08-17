using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面の動作を管理するクラスです。
    /// </summary>
    public class TitleManager : MonoBehaviour, IFadeCallback
    {
        /// <summary>
        /// タイトル画面のメニューを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TitleMenuManager _titleMenuManager;

        /// <summary>
        /// リソースのロードを待つ時間です。
        /// </summary>
        [SerializeField]
        float _resourceLoadWaitTime = 0.5f;

        void Start()
        {
            ResourceLoader.LoadDefinitionData();
            StartCoroutine(StartProcess());
        }

        /// <summary>
        /// ゲーム起動時の処理です。
        /// </summary>
        IEnumerator StartProcess()
        {
            yield return StartCoroutine(WaitLoadResources());

            // 画面をフェードインさせます。
            FadeManager.Instance.SetCallback(this);
            FadeManager.Instance.FadeInScreen();

            // BGMを再生します。
            AudioManager.Instance.PlayBgm(BgmNames.Title);
        }

        /// <summary>
        /// リソースのロードを待ちます。
        /// </summary>
        IEnumerator WaitLoadResources()
        {
            yield return new WaitForSeconds(_resourceLoadWaitTime);
        }

        /// <summary>
        /// フェードが完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedFade()
        {
            // タイトルメニューを操作できるようにします。
            GameStartInfoHolder.isThroughInitScene = true;
            FadeManager.Instance.SetCallback(null);
            _titleMenuManager.StartSelect();
        }
    }
}