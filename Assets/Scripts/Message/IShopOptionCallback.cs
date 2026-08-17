namespace SimpleRpg
{
    /// <summary>
    /// お店用の選択肢ウィンドウのコールバック用インタフェースです。
    /// </summary>
    public interface IShopOptionCallback
    {
        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        void OnSelectedShopOption(int selectedIndex);
    }
}