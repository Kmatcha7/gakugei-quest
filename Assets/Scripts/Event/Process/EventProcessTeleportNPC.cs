using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 指定したNPCを移動させるイベント処理クラスです。
    /// </summary>
    public class EventProcessTeleportNpc : EventProcessBase
    {
        public Transform targetObject;
        [SerializeField] private int X;
        [SerializeField] private int Y;
        [SerializeField] private int Z;
        public override void Execute()
        {
            targetObject.position = new Vector3(X, Y, Z);
            CallNextProcess();
        }


    }
}
