namespace BattleServer.Events
{
    public static class InitEvent
    {
        public enum Params : byte
        {
            Id,
            Wins,
            Loses
        }
    }
}