using Colyseus.Schema;

namespace RealtimeArena.Room
{
    public class GameRoomState : Schema
	{
		[Type(0, "map", typeof(MapSchema<GamePlayer>))]
		public MapSchema<GamePlayer> players = new MapSchema<GamePlayer>();

		[Type(1, "string")]
		public string managerSessionId = string.Empty;

		[Type(2, "uint8")]
		public byte state = 0;
	}
}
