namespace RealtimeArena.Enums
{
    public enum ERoomState : byte
    {
        WaitPlayersToReady = 0,
        CountDownToStartGame = 1,
        WaitPlayersToEnterGame = 2,
        Battle = 3,
        End = 4,
    }
}
