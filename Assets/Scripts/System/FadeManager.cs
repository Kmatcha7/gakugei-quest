using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// 画面のフェードインやフェードアウトを管理するクラスです。
    /// </summary>
    public class FadeManager : MonoBehaviour
    {
        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        private static FadeManager _instance;

        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        public static FadeManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 画面フェードに使用するImageコンポーネントです。
        /// </summary>
        [SerializeField]
        Image _fadeImage;

        /// <summary>
        /// フェードのコールバックを行うインターフェースです。
        /// </summary>
        IFadeCallback _fadeCallback;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// コールバック先を登録します。
        /// </summary>
        /// <param name="fadeCallback">フェード完了時に呼び出されるコールバック</param>
        public void SetCallback(IFadeCallback fadeCallback)
        {
            _fadeCallback = fadeCallback;
        }

        /// <summary>
        /// 画面をフェードインさせます。
        /// </summary>
        /// <param name="fadeTime">フェードインにかかる時間（秒）</param>
        public void FadeInScreen(float fadeTime = 0.5f)
        {
            if (_fadeImage == null)
            {
                SimpleLogger.Instance.LogError("FadeManagerの_fadeImageが設定されていません。");
                return;
            }

            _fadeImage.raycastTarget = false;
            var sourceColor = Color.black;
            var targetColor = Color.clear;
            StartCoroutine(MovePlayerProcess(sourceColor, targetColor, fadeTime));
        }

        /// <summary>
        /// 画面をフェードアウトさせます。
        /// </summary>
        /// <param name="fadeTime">フェードアウトにかかる時間（秒）</param>
        public void FadeOutScreen(float fadeTime = 0.5f)
        {
            if (_fadeImage == null)
            {
                SimpleLogger.Instance.LogError("FadeManagerの_fadeImageが設定されていません。");
                return;
            }

            _fadeImage.raycastTarget = true;
            var sourceColor = Color.clear;
            var targetColor = Color.black;
            StartCoroutine(MovePlayerProcess(sourceColor, targetColor, fadeTime));
        }

        /// <summary>
        /// Imageの色を変化させるコルーチンです。
        /// </summary>
        /// <param name="sourceColor">開始色</param>
        /// <param name="targetColor">終了色</param>
        /// <param name="animTime">アニメーション時間</param>
        IEnumerator MovePlayerProcess(Color sourceColor, Color targetColor, float animTime)
        {
            // 完了後の時間を算出して、それまでの間、毎フレームLerpメソッドで透明度を計算します。
            var animFinishTime = Time.time + animTime;
            var startedTime = Time.time;
            while (Time.time < animFinishTime)
            {
                var elapsedTime = Time.time - startedTime;
                var rate = Mathf.Clamp01(elapsedTime / animTime);
                Color color = Color.Lerp(sourceColor, targetColor, rate);
                _fadeImage.color = color;
                yield return null;
            }

            // 最終的な色を設定します。
            _fadeImage.color = targetColor;

            PostFade();
        }

        /// <summary>
        /// フェード完了後の処理です。
        /// </summary>
        void PostFade()
        {
            if (_fadeCallback != null)
            {
                _fadeCallback.OnFinishedFade();
            }
        }
    }
}