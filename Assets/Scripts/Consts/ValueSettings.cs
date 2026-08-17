namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内の数値に関する上限値や定義値です。
    /// </summary>
    public static class ValueSettings
    {
        /// <summary>
        /// HPの上限値です。
        /// </summary>
        public const int MaxHp = 999;

        /// <summary>
        /// MPの上限値です。
        /// </summary>
        public const int MaxMp = 999;

        /// <summary>
        /// アイテム所持数の上限値です。
        /// </summary>
        public const int MaxItemNum = 99;

        /// <summary>
        /// 所持金の上限値です。
        /// </summary>
        public const int MaxGold = 999999;

        /// <summary>
        /// アイテムの売値の倍率です。
        /// </summary>
        public const float SellPriceMultiplier = 0.5f;
    }
}