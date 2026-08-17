using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// セーブ機能のテストを行うためのクラスです。
    /// </summary>
    public class SaveTester : MonoBehaviour
    {
        /// <summary>
        /// セーブデータの管理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveDataManager _saveDataManager;

        /// <summary>
        /// セーブデータをロードするかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _executeLoadData;

        /// <summary>
        /// セーブデータにセーブするかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _executeSaveData;

        /// <summary>
        /// データを読み書きするセーブ枠のIDです。
        /// </summary>
        [SerializeField]
        int _slotId = 1;

        void Start()
        {

        }

        void Update()
        {
            // 定義データのロードを待つため、最初の5フレームは処理を抜けます。
            if (Time.frameCount < 5)
            {
                return;
            }

            if (_executeLoadData)
            {
                _executeLoadData = false;
                _saveDataManager.SetLoadedData(_slotId);
                return;
            }

            if (_executeSaveData)
            {
                _executeSaveData = false;
                _saveDataManager.SaveDataToFile(_slotId);
                return;
            }
        }
    }
}