using Colyseus.Schema;

namespace RealtimeArena.Room
{
    public class LobbyRoomState : Schema
	{
		[Type(0, "map", typeof(MapSchema<LobbyPlayer>))]
		public MapSchema<LobbyPlayer> players = new MapSchema<LobbyPlayer>();

		[Type(1, "string")]
		public string title = default(string);

		[Type(2, "boolean")]
		public bool hasPassword = default(bool);
	}
}
