namespace SimpleRpg
{
    /// <summary>
    /// ゲームを開始する際の情報を保持するクラスです。
    /// </summary>
    public static class GameStartInfoHolder
    {
        /// <summary>
        /// はじめからゲームを開始するかどうかを示すフラグです。
        /// </summary>
        public static bool isNewGame = true;

        /// <summary>
        /// つづきからゲームを開始する場合のセーブデータのスロットIDです。
        /// </summary>
        public static int loadedSlotId = -1;

        /// <summary>
        /// 初期化用のシーンを通ったかどうかを示すフラグです。
        /// </summary>
        public static bool isThroughInitScene;
    }
}