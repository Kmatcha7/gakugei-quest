using UnityEngine;

namespace SimpleRpg
{
    
    /// <summary>
    /// イベントのページでフラグに関する条件を確認するクラスです。
    /// </summary>
    public class EventProcessDestroyGameObject : EventProcessBase
    {
        public GameObject targetObject;
        public override void Execute()
        {

            Destroy(targetObject);
            CallNextProcess();
        }

    }
}