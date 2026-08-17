using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// カメラを対象のキャラクターに追従させる処理を行うクラスです。
    /// </summary>
    public class CameraMover : MonoBehaviour
    {
        /// <summary>
        /// 追従対象のゲームオブジェクトへの参照です。
        /// </summary>
        GameObject _targetObj;

        /// <summary>
        /// 追従しているかどうかのフラグです。
        /// </summary>
        bool _isTracing;

        void Start()
        {
            CheckReferences();
            StartTrace();
        }

        /// <summary>
        /// 必要な参照を確認します。
        /// </summary>
        void CheckReferences()
        {
            if (_targetObj == null)
            {
                _targetObj = GameObject.FindWithTag(ObjectTagSettings.Player);
            }
        }

        /// <summary>
        /// カメラの追従を開始します。
        /// </summary>
        public void StartTrace()
        {
            _isTracing = true;
        }

        /// <summary>
        /// カメラの追従を停止します。
        /// </summary>
        public void StopTrace()
        {
            _isTracing = false;
        }

        void LateUpdate()
        {
            MoveCamera();
        }

        /// <summary>
        /// カメラを移動させる処理です。
        /// </summary>
        void MoveCamera()
        {
            // 追跡中フラグがfalseなら処理を抜けます。
            if (!_isTracing)
            {
                return;
            }

            // 対象キャラクターへの参照がnullなら抜けます。
            if (_targetObj == null)
            {
                return;
            }

            // 対象キャラクターの位置までカメラを移動させます。この際、Z軸の位置は変更しません。
            var cameraPos = _targetObj.transform.position;
            cameraPos.z = gameObject.transform.position.z;
            gameObject.transform.position = cameraPos;
        }
    }
}