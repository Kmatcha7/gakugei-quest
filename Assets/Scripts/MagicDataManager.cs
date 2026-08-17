using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内の魔法データを管理するクラスです。
    /// </summary>
    public static class MagicDataManager
    {
        /// <summary>
        /// 読み込んだ魔法データの一覧です。
        /// </summary>
        static List<MagicData> _magicDataList = new();

        /// <summary>
        /// 魔法データをロードします。
        /// </summary>
        public static async void LoadMagicData()
        {
            AsyncOperationHandle<IList<MagicData>> handle = Addressables.LoadAssetsAsync<MagicData>(AddressablesLabels.Magic, null);
            await handle.Task;
            _magicDataList = new List<MagicData>(handle.Result);
        }

        /// <summary>
        /// IDから魔法データを取得します。
        /// </summary>
        public static MagicData GetMagicDataById(int magicId)
        {
            return _magicDataList.Find(magic => magic.magicId == magicId);
        }

        /// <summary>
        /// 全てのデータを取得します。
        /// </summary>
        public static List<MagicData> GetAllData()
        {
            return _magicDataList;
        }
        
        public static List<MagicData> GetUnlockedMagicList()
        {    
            List<MagicData> unlockedList = new();

            foreach (var magic in _magicDataList)
            {
                if (!string.IsNullOrEmpty(magic.unlockFlagName) &&
                    FlagManager.Instance.GetFlagState(magic.unlockFlagName))
                {
                    unlockedList.Add(magic);
                }
            }
            return unlockedList;
        }
    }
}