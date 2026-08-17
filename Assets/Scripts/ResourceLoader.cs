namespace SimpleRpg
{
    /// <summary>
    /// 定義データをロードするクラスです。
    /// </summary>
    public static class ResourceLoader
    {
        /// <summary>
        /// 定義データをロードします。
        /// </summary>
        public static void LoadDefinitionData()
        {
            CharacterDataManager.LoadCharacterData();
            CharacterDataManager.LoadExpTables();
            CharacterDataManager.LoadParameterTables();

            EnemyDataManager.LoadEnemyData();
            ItemDataManager.LoadItemData();
            MagicDataManager.LoadMagicData();
            MapDataManager.LoadMapData();

            SaveDataHolder.Load();
        }
    }
}