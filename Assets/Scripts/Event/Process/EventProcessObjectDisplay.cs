using UnityEngine;

namespace SimpleRpg
{
    
    /// <summary>
    /// イベントのページでフラグに関する条件を確認するクラスです。
    /// </summary>
    public class EventProcessObjectDisplay : EventProcessBase
    {

        [SerializeField]  bool _SetActive = true;
        [SerializeField] private Renderer targetObject;
        public override void Execute()
        {
            if(_SetActive == true)
            {
                targetObject.enabled = true;
                Debug.Log("true");
            }
            else
            {
                targetObject.enabled = false;
                Debug.Log("false");
            }
        
        CallNextProcess();
        }

    }
}