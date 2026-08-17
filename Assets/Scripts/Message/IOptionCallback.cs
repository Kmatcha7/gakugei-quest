namespace SimpleRpg
{
    /// <summary>
    /// 選択肢ウィンドウのコールバック用インタフェースです。
    /// </summary>
    public interface IOptionCallback
    {
        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        void OnSelectedOption(int selectedIndex);
    }
}