using Colyseus.Schema;

namespace RealtimeArena.Room
{
    public class GameRoomState : Schema
	{
		[Type(0, "map", typeof(MapSchema<GamePlayer>))]
		public MapSchema<GamePlayer> players = new MapSchema<GamePlayer>();

		[Type(1, "boolean")]
		public bool isStarting = default(bool);
	}
}
