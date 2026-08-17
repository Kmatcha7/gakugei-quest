using UnityEngine;
using System;

namespace SimpleRpg
{
    /// <summary>
    /// セーブデータの管理を行うクラスです。
    /// </summary>
    [Serializable]
    public class SaveDataManager : MonoBehaviour
    {
        /// <summary>
        /// セーブデータとステータス情報をやりとりするクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveInfoStatusController _saveInfoStatusController;

        /// <summary>
        /// セーブデータとマップ情報をやりとりするクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveInfoMapController _saveInfoMapController;

        /// <summary>
        /// セーブデータとフラグ情報をやりとりするクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveInfoFlagController _saveInfoFlagController;

        /// <summary>
        /// ロードした情報について、指定したセーブ枠の情報をメモリに読み込みます。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        public void SetLoadedData(int slotId)
        {
            if (!SaveDataUtil.IsValidSlotId(slotId))
            {
                SimpleLogger.Instance.LogError($"ロードに失敗しました。セーブ枠のIDが無効です。 ID : {slotId}");
                return;
            }

            if (SaveDataHolder.SaveFile == null)
            {
                SimpleLogger.Instance.LogWarning($"ロードに失敗しました。読み込んだセーブファイルが存在しません。");
                return;
            }

            if (SaveDataHolder.SaveFile.saveSlots == null)
            {
                SimpleLogger.Instance.LogWarning($"ロードに失敗しました。セーブファイルのセーブ枠が存在しません。");
                return;
            }

            var saveSlot = SaveDataHolder.SaveFile.saveSlots.Find(slot => slot.slotId == slotId);
            if (saveSlot == null)
            {
                SimpleLogger.Instance.LogWarning($"ロードに失敗しました。IDに対応するデータが存在しませんでした。 ID : {slotId}");
                return;
            }

            // セーブ枠の情報をメモリに読み込みます。
            _saveInfoStatusController.SetSaveInfoStatus(saveSlot.saveInfoStatus);
            _saveInfoMapController.SetSaveInfoMap(saveSlot.saveInfoMap);
            _saveInfoFlagController.SetSaveInfoFlag(saveSlot.saveInfoFlag);
        }

        /// <summary>
        /// ゲーム内の実行中データをセーブファイルに保存します。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        public void SaveDataToFile(int slotId)
        {
            if (!SaveDataUtil.IsValidSlotId(slotId))
            {
                SimpleLogger.Instance.LogError($"セーブに失敗しました。セーブ枠のIDが無効です。 ID : {slotId}");
                return;
            }

            UpdateSlotData(slotId);
            SaveDataHolder.Save();
        }

        /// <summary>
        /// 指定した枠のデータをメモリ上から取得します。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        void UpdateSlotData(int slotId)
        {
            var saveInfoStatus = _saveInfoStatusController.GetSaveInfoStatus();
            var saveInfoMap = _saveInfoMapController.GetSaveInfoMap();
            var saveInfoFlag = _saveInfoFlagController.GetSaveInfoFlag();

            if (SaveDataHolder.SaveFile == null)
            {
                SaveDataHolder.InitializeSaveFile();
            }

            if (SaveDataHolder.SaveFile.saveSlots == null)
            {
                SaveDataHolder.InitializeSaveFile();
            }

            var saveSlot = SaveDataHolder.SaveFile.saveSlots.Find(slot => slot.slotId == slotId);
            if (saveSlot == null)
            {
                saveSlot = GetSlotData(slotId);
                SaveDataHolder.SaveFile.saveSlots.Add(saveSlot);
            }
            else
            {
                saveSlot.saveInfoStatus = saveInfoStatus;
                saveSlot.saveInfoMap = saveInfoMap;
                saveSlot.saveInfoFlag = saveInfoFlag;
            }
        }

        /// <summary>
        /// 枠の情報を取得します。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        SaveSlot GetSlotData(int slotId)
        {
            var saveInfoStatus = _saveInfoStatusController.GetSaveInfoStatus();
            var saveInfoMap = _saveInfoMapController.GetSaveInfoMap();
            var saveInfoFlag = _saveInfoFlagController.GetSaveInfoFlag();

            SaveSlot saveSlot = new()
            {
                slotId = slotId,
                saveInfoStatus = saveInfoStatus,
                saveInfoMap = saveInfoMap,
                saveInfoFlag = saveInfoFlag
            };
            return saveSlot;
        }

        /// <summary>
        /// 指定した枠のデータを取得します。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        public SaveSlot GetSaveSlot(int slotId)
        {
            SaveSlot saveSlot = null;
            if (SaveDataHolder.SaveFile == null)
            {
                SimpleLogger.Instance.LogWarning($"セーブデータが存在しません。");
                return saveSlot;
            }

            if (SaveDataHolder.SaveFile.saveSlots == null)
            {
                SimpleLogger.Instance.LogWarning($"セーブデータのセーブ枠が存在しません。");
                return saveSlot;
            }
            saveSlot = SaveDataHolder.SaveFile.saveSlots.Find(slot => slot.slotId == slotId);
            return saveSlot;
        }
    }
}