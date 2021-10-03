using RealtimeArena.Room;
using System;
using UnityEngine.Events;

namespace RealtimeArena.Event
{
    /// <summary>
    /// [GameRoomState state, bool isFirstState]
    /// </summary>
    [Serializable]
    public class RoomStateChangeEvent : UnityEvent<GameRoomState, bool>
    {
    }
}
