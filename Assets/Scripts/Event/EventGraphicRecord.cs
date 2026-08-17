using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 条件とイベントの画像の対応を保持するクラスです。
    /// </summary>
    public class EventGraphicRecord : MonoBehaviour
    {
        /// <summary>
        /// 対応する画像やアニメーターを表示するイベントページです。
        /// </summary>
        public List<EventPage> eventPages;

        /// <summary>
        /// 条件に対応する画像です。
        /// </summary>
        public Sprite sprite;

        /// <summary>
        /// 条件に対応するアニメーターです。
        /// </summary>
        public RuntimeAnimatorController animatorController;

        /// <summary>
        /// アニメーターを止めるかどうかのフラグです。
        /// </summary>
        public bool isStopAnimator;
    }
}