using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内のマップ情報の定義データを管理するクラスです。
    /// </summary>
    public static class MapDataManager
    {
        /// <summary>
        /// 読み込んだマップデータの一覧です。
        /// </summary>
        static List<MapData> _mapDataList = new();

        /// <summary>
        /// マップデータをロードします。
        /// </summary>
        public static async void LoadMapData()
        {
            AsyncOperationHandle<IList<MapData>> handle = Addressables.LoadAssetsAsync<MapData>(AddressablesLabels.MapData, null);
            await handle.Task;
            _mapDataList = new List<MapData>(handle.Result);
        }

        /// <summary>
        /// IDからマップデータを取得します。
        /// </summary>
        /// <param name="mapId">マップID</param>
        public static MapDataRecord GetMapDataRecordById(int mapId)
        {
            if (_mapDataList == null || _mapDataList.Count == 0)
            {
                return null;
            }

            var mapData = _mapDataList[0];
            if (mapData.mapDataRecords == null)
            {
                return null;
            }
            return mapData.mapDataRecords.Find(item => item.mapId == mapId);
        }

        /// <summary>
        /// マップIDからマップ名を取得します。
        /// </summary>
        /// <param name="mapId">マップID</param>
        public static string GetMapName(int mapId)
        {
            string mapName = string.Empty;
            var mapDataRecord = GetMapDataRecordById(mapId);
            if (mapDataRecord != null)
            {
                mapName = mapDataRecord.name;
            }
            return mapName;
        }

        /// <summary>
        /// マップIDからマップのBGM名を取得します。
        /// </summary>
        /// <param name="mapId">マップID</param>
        public static string GetMapBgmName(int mapId)
        {
            string bgmName = string.Empty;
            var mapDataRecord = GetMapDataRecordById(mapId);
            if (mapDataRecord != null)
            {
                bgmName = mapDataRecord.bgmName;
            }
            return bgmName;
        }
    }
}