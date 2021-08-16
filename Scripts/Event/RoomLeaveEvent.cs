using System;
using UnityEngine.Events;

namespace RealtimeArena.Event
{
    /// <summary>
    /// [int code]
    /// </summary>
    [Serializable]
    public class RoomLeaveEvent : UnityEvent<int>
    {
    }
}
