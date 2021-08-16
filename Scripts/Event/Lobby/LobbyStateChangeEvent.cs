using RealtimeArena.Room;
using System;
using UnityEngine.Events;

namespace RealtimeArena.Event.Lobby
{
    /// <summary>
    /// [LobbyRoomState state, bool isFirstState]
    /// </summary>
    [Serializable]
    public class LobbyStateChangeEvent : UnityEvent<LobbyRoomState, bool>
    {
    }
}
