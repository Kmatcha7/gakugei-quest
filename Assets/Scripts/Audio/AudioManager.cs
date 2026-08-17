using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// オーディオを管理するクラスです。
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        private static AudioManager _instance;

        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<AudioManager>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new();
                        _instance = singletonObject.AddComponent<AudioManager>();
                        singletonObject.name = typeof(AudioManager).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// BGMを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BgmManager _bgmManager;

        /// <summary>
        /// SEを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SeManager _seManager;

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
        /// 参照をセットします。
        /// </summary>
        public void SetUpReferences()
        {
            // BGMManagerの参照を取得します。
            if (_bgmManager == null)
            {
                _bgmManager = GetComponentInChildren<BgmManager>();

                // 子オブジェクトにBgmManagerが見つからない場合、新しいGameObjectを作成します。
                if (_bgmManager == null)
                {
                    string objectName = nameof(BgmManager);
                    GameObject bgmObject = new(objectName);
                    _bgmManager = bgmObject.AddComponent<BgmManager>();
                }
            }

            // SeManagerの参照を取得します。
            if (_seManager == null)
            {
                _seManager = GetComponentInChildren<SeManager>();

                // 子オブジェクトにSeManagerが見つからない場合、新しいGameObjectを作成します。
                if (_seManager == null)
                {
                    string objectName = nameof(SeManager);
                    GameObject seObject = new(objectName);
                    _seManager = seObject.AddComponent<SeManager>();
                }
            }
        }

        /// <summary>
        /// BGMを再生します。
        /// </summary>
        public void PlayBgm(string bgmName, bool isResume = false, bool isLoop = true)
        {
            SetUpReferences();
            _bgmManager.Play(bgmName, isResume, isLoop);
        }

        /// <summary>
        /// 指定したBGMを停止します。
        /// </summary>
        public void StopBgm(string bgmName, float fadeTime = 0.25f)
        {
            SetUpReferences();
            _bgmManager.Stop(bgmName, fadeTime);
        }

        /// <summary>
        /// 全てのBGMを停止します。
        /// </summary>
        public void StopAllBgm(float fadeTime = 0.25f)
        {
            SetUpReferences();
            _bgmManager.StopAllBgm(fadeTime);
        }

        /// <summary>
        /// 効果音を再生します。
        /// </summary>
        public void PlaySe(string seName, bool isLoop = false)
        {
            SetUpReferences();
            _seManager.Play(seName, isLoop);
        }

        /// <summary>
        /// 全ての効果音を停止します。
        /// </summary>
        public void StopAllSe(float fadeTime = 0.25f)
        {
            SetUpReferences();
            _seManager.StopAllSe(fadeTime);
        }

        /// <summary>
        /// 指定されたAudioSourceで再生中のオーディオをフェードさせます。
        /// </summary>
        /// <param name="audioSource">フェードさせるAudioSource</param>
        /// <param name="targetVolume">目標音量</param>
        /// <param name="fadeTime">フェード時間</param>
        public IEnumerator FadeAudio(AudioSource audioSource, float targetVolume, float fadeTime)
        {
            // 完了後の時間を算出して、それまでの間、毎フレームLerpメソッドで音量を計算します。
            var animFinishTime = Time.time + fadeTime;
            var startedTime = Time.time;
            var startVolume = audioSource.volume;
            while (Time.time < animFinishTime)
            {
                var elapsedTime = Time.time - startedTime;
                var rate = Mathf.Clamp01(elapsedTime / fadeTime);
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, rate);
                yield return null;
            }

            // 最終的な音量を設定します。
            audioSource.volume = targetVolume;
        }
    }
}