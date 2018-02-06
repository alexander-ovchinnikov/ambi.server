namespace BattleServer.Events
{
    internal static class BattleEvent
    {
        public enum Params : byte
        {
            PlayerHealth,
            EnemyHealth
        }
    }
}