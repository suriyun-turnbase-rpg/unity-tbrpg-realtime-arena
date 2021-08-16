using Colyseus.Schema;

namespace RealtimeArena.Room
{
	public partial class LobbyPlayer : Schema
	{
		[Type(0, "string")]
		public string playerName = default(string);

		[Type(1, "int32")]
		public int playerLevel = default(int);

		[Type(2, "int32")]
		public int teamBP = default(int);

		[Type(3, "string")]
		public string leaderCharacterId = default(string);

		[Type(4, "int32")]
		public int leaderCharacterLevel = default(int);

		[Type(5, "boolean")]
		public bool isReady = default(bool);
	}
}
