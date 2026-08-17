namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関する状態を定義する列挙型です。
    /// </summary>
    public enum BattlePhase
    {
        NotInBattle,
        ShowEnemy,
        InputCommand,
        SelectItem,
        Action,
        Result,
    }
}