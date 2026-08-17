using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// オーディオのチャンネル情報を保持するクラスです。
    /// </summary>
    public class AudioChannelInfo
    {
        /// <summary>
        /// オーディオのチャンネルIDです。
        /// </summary>
        public int channelId;

        /// <summary>
        /// 再生中のオーディオ名です。
        /// </summary>
        public string playingAudioName;

        /// <summary>
        /// 対応するAudioSourceです。
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// AudioSourceが再生中かどうかを返します。
        /// </summary>
        public bool IsPlaying()
        {
            return audioSource != null && audioSource.isPlaying;
        }
    }
}