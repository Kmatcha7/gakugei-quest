using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// BGMを管理するクラスです。
    /// </summary>
    public class BgmManager : MonoBehaviour
    {
        /// <summary>
        /// チャンネルの情報を保持するリストです。
        /// </summary>
        List<AudioChannelInfo> _audioChannelInfoList = new();

        /// <summary>
        /// BGM名と再生位置を保持する辞書です。
        /// </summary>
        Dictionary<string, float> _bgmPlaybackPositions = new();

        /// <summary>
        /// BGMの音量です。
        /// </summary>
        readonly float Volume = 0.75f;

        /// <summary>
        /// チャンネル用ゲームオブジェクトの名前の接頭辞です。
        /// </summary>
        readonly string ChannelPrefix = "BGM Channel";

        /// <summary>
        /// BGMのチャンネル数です。
        /// </summary>
        readonly int ChannelCount = 2;

        /// <summary>
        /// BGMを再生します。
        /// </summary>
        /// <param name="bgmName">再生するBGMの名前</param>
        /// <param name="isResume">再生位置を保持しているかどうか</param>
        /// <param name="isLoop">ループ再生するかどうか</param>
        public void Play(string bgmName, bool isResume = false, bool isLoop = true)
        {
            SetUpAudioSource();
            var channelInfo = GetAudioChannelInfo();
            if (channelInfo == null)
            {
                SimpleLogger.Instance.LogError("再生可能なチャンネル情報が取得できませんでした。");
                return;
            }

            var audioSource = channelInfo.audioSource;
            if (audioSource == null)
            {
                SimpleLogger.Instance.LogError("チャンネルに対応するAudioSourceが取得できませんでした。");
                return;
            }

            // BGMのAudioClipを取得します。
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(bgmName);
            handle.Completed += (operation) =>
            {
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    audioSource.clip = operation.Result;
                    audioSource.loop = isLoop;
                    audioSource.volume = Volume;
                    if (isResume)
                    {
                        audioSource.time = GetPlaybackPosition(bgmName);
                    }
                    audioSource.Play();

                    channelInfo.playingAudioName = bgmName;
                }
                else
                {
                    SimpleLogger.Instance.LogError($"指定されたBGMが見つかりません: {bgmName}");
                }
            };
        }

        /// <summary>
        /// BGMを停止します。
        /// </summary>
        /// <param name="bgmName">停止するBGMの名前</param>
        /// <param name="fadeTime">フェードアウトにかかる時間</param>
        public void Stop(string bgmName = "", float fadeTime = 0.25f)
        {
            var channelInfo = _audioChannelInfoList.Find(info => info.playingAudioName == bgmName);
            if (channelInfo == null || channelInfo.audioSource == null)
            {
                SimpleLogger.Instance.LogWarning($"指定されたBGMが再生されていません: {bgmName}");
                return;
            }
            StartCoroutine(StopBgmProcess(bgmName, channelInfo.audioSource, fadeTime));
        }

        /// <summary>
        /// 全てのBGMを停止します。
        /// </summary>
        /// <param name="fadeTime">フェードアウトにかかる時間</param>
        public void StopAllBgm(float fadeTime = 0.25f)
        {
            foreach (var channelInfo in _audioChannelInfoList)
            {
                if (channelInfo.audioSource == null)
                {
                    continue;
                }

                StartCoroutine(StopBgmProcess(channelInfo.playingAudioName, channelInfo.audioSource, fadeTime));
            }
        }

        /// <summary>
        /// BGMを停止します。
        /// </summary>
        /// <param name="fadeTime">フェードアウトにかかる時間</param>
        public IEnumerator StopBgmProcess(string bgmName, AudioSource audioSource, float fadeTime = 0.25f)
        {
            yield return StartCoroutine(AudioManager.Instance.FadeAudio(audioSource, 0f, fadeTime));
            KeepPlaybackPosition(bgmName, audioSource);
            audioSource.Stop();
        }

        /// <summary>
        /// 指定したBGM名のオーディオの再生位置を辞書に保持します。
        /// </summary>
        /// <param name="bgmName">BGMの名前</param>
        /// <param name="audioSource">再生中のAudioSource</param>
        public void KeepPlaybackPosition(string bgmName, AudioSource audioSource)
        {
            if (audioSource == null)
            {
                return;
            }

            if (audioSource.clip == null)
            {
                return;
            }

            if (_bgmPlaybackPositions.ContainsKey(bgmName))
            {
                _bgmPlaybackPositions[bgmName] = audioSource.time;
            }
            else
            {
                _bgmPlaybackPositions.Add(bgmName, audioSource.time);
            }
        }

        /// <summary>
        /// 指定したBGM名のオーディオの再生位置を取得します。
        /// </summary>
        /// <param name="bgmName">BGMの名前</param>
        float GetPlaybackPosition(string bgmName)
        {
            _bgmPlaybackPositions.TryGetValue(bgmName, out float position);
            return position;
        }

        /// <summary>
        /// AudioSourceのセットアップを行います。
        /// </summary>
        void SetUpAudioSource()
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                // リストに登録されていないチャンネルであれば新しいAudioChannelInfoを生成します。
                if (!_audioChannelInfoList.Exists(info => info.channelId == i))
                {
                    string objectName = $"{ChannelPrefix} {i}";
                    GameObject audioSourceObject = new (objectName);
                    audioSourceObject.transform.SetParent(transform);
                    AudioSource newAudioSource = audioSourceObject.AddComponent<AudioSource>();
                    
                    AudioChannelInfo audioChannelInfo = new()
                    {
                        channelId = i,
                        audioSource = newAudioSource,
                        playingAudioName = string.Empty,
                    };
                    _audioChannelInfoList.Add(audioChannelInfo);
                }
            }
        }

        /// <summary>
        /// オーディオを再生するチャンネル情報を取得します。
        /// </summary>
        public AudioChannelInfo GetAudioChannelInfo()
        {
            var channelId = GetStoppedAudioSourceChannelId();
            return _audioChannelInfoList.Find(info => info.channelId == channelId);
        }

        /// <summary>
        /// 停止しているAudioSourceがあるチャンネルIDを取得します。
        /// </summary>
        public int GetStoppedAudioSourceChannelId()
        {
            foreach (var channelInfo in _audioChannelInfoList)
            {
                if (!channelInfo.IsPlaying())
                {
                    return channelInfo.channelId;
                }
            }

            // すべてのAudioSourceが再生中の場合、最初のAudioSourceを返します。
            return 0;
        }
    }
}