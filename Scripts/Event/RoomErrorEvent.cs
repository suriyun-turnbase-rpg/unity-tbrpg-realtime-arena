using System;
using UnityEngine.Events;

namespace RealtimeArena.Event
{
    /// <summary>
    /// [int code, string message]
    /// </summary>
    [Serializable]
    public class RoomErrorEvent : UnityEvent<int, string>
    {
    }
}
