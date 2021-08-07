using Colyseus;
using System;

namespace RealtimeArena.Room
{
    [Serializable]
    public class LoobyRoomAvailable : ColyseusRoomAvailable
    {
        public string title;
        public bool hasPassword;
    }
}
