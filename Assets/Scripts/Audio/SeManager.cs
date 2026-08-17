using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// 効果音を管理するクラスです。
    /// </summary>
    public class SeManager : MonoBehaviour
    {
        /// <summary>
        /// チャンネルの情報を保持するリストです。
        /// </summary>
        List<AudioChannelInfo> _audioChannelInfoList = new();

        /// <summary>
        /// 効果音の音量です。
        /// </summary>
        readonly float Volume = 0.75f;

        /// <summary>
        /// チャンネル用ゲームオブジェクトの名前の接頭辞です。
        /// </summary>
        readonly string ChannelPrefix = "SE Channel";

        /// <summary>
        /// 効果音を再生します。
        /// </summary>
        /// <param name="seName">再生する効果音の名前</param>
        /// <param name="isLoop">ループ再生するかどうか</param>
        public void Play(string seName, bool isLoop = false)
        {
            // 効果音のアドレスを取得します。
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(seName);
            handle.Completed += (operation) =>
            {
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    AudioSource audioSource = GetAudioSource();
                    audioSource.clip = operation.Result;
                    audioSource.loop = isLoop;
                    audioSource.volume = Volume;
                    audioSource.Play();
                }
                else
                {
                    SimpleLogger.Instance.LogError($"指定された効果音が見つかりません: {seName}");
                }
            };
        }

        /// <summary>
        /// 全ての効果音を停止します。
        /// </summary>
        /// <param name="fadeTime">フェードアウトにかかる時間</param>
        public void StopAllSe(float fadeTime = 0.25f)
        {
            foreach (var channelInfo in _audioChannelInfoList)
            {
                if (channelInfo.audioSource == null)
                {
                    continue;
                }

                StartCoroutine(StopSeProcess(channelInfo.playingAudioName, channelInfo.audioSource, fadeTime));
            }
        }

        /// <summary>
        /// BGMを停止します。
        /// </summary>
        /// <param name="fadeTime">フェードアウトにかかる時間</param>
        public IEnumerator StopSeProcess(string seName, AudioSource audioSource, float fadeTime = 0f)
        {
            yield return StartCoroutine(AudioManager.Instance.FadeAudio(audioSource, 0f, fadeTime));
            audioSource.Stop();
        }

        /// <summary>
        /// オーディオを再生するAudioSourceを取得します。
        /// </summary>
        public AudioSource GetAudioSource()
        {
            var audioSource = GetStoppedAudioSource();
            if (audioSource != null)
            {
                return audioSource;
            }

            // すべてのAudioSourceが再生中の場合、新しいAudioSourceを生成します。
            GameObject audioSourceObject = new(ChannelPrefix);
            audioSourceObject.transform.SetParent(transform);
            AudioSource newAudioSource = audioSourceObject.AddComponent<AudioSource>();
            AudioChannelInfo audioChannelInfo = new()
            {
                channelId = GetNewChannelId(),
                audioSource = newAudioSource,
                playingAudioName = string.Empty,
            };
            _audioChannelInfoList.Add(audioChannelInfo);
            return newAudioSource;
        }

        /// <summary>
        /// 停止しているAudioSourceを取得します。
        /// </summary>
        public AudioSource GetStoppedAudioSource()
        {
            foreach (var audioChannelInfo in _audioChannelInfoList)
            {
                if (!audioChannelInfo.IsPlaying())
                {
                    return audioChannelInfo.audioSource;
                }
            }
            return null;
        }

        /// <summary>
        /// 新しいチャンネルIDを取得します。
        /// </summary>
        int GetNewChannelId()
        {
            int newChannelId = 0;
            while (_audioChannelInfoList.Exists(info => info.channelId == newChannelId))
            {
                newChannelId++;
            }
            return newChannelId;
        }
    }
}