using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// セーブデータに関する処理を行う便利クラスです。
    /// </summary>
    public static class SaveDataUtil
    {
        /// <summary>
        /// 引数のセーブ枠のIDが有効かどうか確認します。
        /// セーブ枠のIDは1から開始します。
        /// </summary>
        /// <param name="slotId">対象の枠のID</param>
        public static bool IsValidSlotId(int slotId)
        {
            return slotId >= 1 && slotId <= SaveSettings.SlotNum;
        }

        /// <summary>
        /// セーブファイルについてファイル名を含めたパスを取得します。
        /// </summary>
        public static string GetSaveFilePath()
        {
            string path = Application.persistentDataPath;
            string fullPath = @$"{path}/{SaveSettings.SaveFileName}";
            return fullPath;
        }
    }
}