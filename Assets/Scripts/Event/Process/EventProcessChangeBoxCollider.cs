using UnityEngine;

namespace SimpleRpg
{
    
    /// <summary>
    /// イベントのページでフラグに関する条件を確認するクラスです。
    /// </summary>
    public class EventProcessChangeBoxCollider : EventProcessBase
    {

        [SerializeField] private float sizeX;
        [SerializeField] private float sizeY;
        [SerializeField] private float offsetX;
        [SerializeField] private float offsetY;
        public BoxCollider2D targetBox;
        public override void Execute()
        {

        // サイズ変更
        targetBox.size = new Vector2(sizeX, sizeY);

        // 位置（オフセット）変更
        targetBox.offset = new Vector2(offsetX, offsetY);
        CallNextProcess();
        }

    }
}