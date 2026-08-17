using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面のゲームの終了メニューを制御するクラスです。
    /// </summary>
    public class TitleQuitGameController : MonoBehaviour, IFadeCallback
    {
        /// <summary>
        /// ゲームを終了します。
        /// </summary>
        public void QuitGame()
        {
            FadeManager.Instance.SetCallback(this);
            FadeManager.Instance.FadeOutScreen();
        }

        /// <summary>
        /// フェードが完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedFade()
        {
#if UNITY_EDITOR
                // エディタ上ではプレイモードを終了します。
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // 実行環境ではアプリケーションを終了します。
                Application.Quit();
#endif
        }
    }
}