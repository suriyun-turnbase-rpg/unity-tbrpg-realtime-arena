using Colyseus.Schema;

namespace RealtimeArena.Room
{
    public class LobbyRoomState : Schema
	{
		[Type(0, "map", typeof(MapSchema<LobbyPlayer>))]
		public MapSchema<LobbyPlayer> players = new MapSchema<LobbyPlayer>();
	}
}
