namespace SimpleRpg
{
    /// <summary>
    /// セーブに関する設定の定義です。
    /// </summary>
    public static class SaveSettings
    {
        /// <summary>
        /// セーブ枠の数です。
        /// </summary>
        public const int SlotNum = 3;

        /// <summary>
        /// セーブファイルの名前です。
        /// </summary>
        public const string SaveFileName = "SimpleSave.json";

        /// <summary>
        /// WebGLのセーブデータのキー名です。
        /// </summary>
        public const string WebGLSaveKey = "SimpleRpgSaveData";
    }
}