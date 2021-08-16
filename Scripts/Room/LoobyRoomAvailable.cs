using Colyseus;
using System;

namespace RealtimeArena.Room
{
    [Serializable]
    public class LoobyRoomAvailable : ColyseusRoomAvailable
    {
        public LobbyRoomAvailableMetadata metadata;
    }
}
