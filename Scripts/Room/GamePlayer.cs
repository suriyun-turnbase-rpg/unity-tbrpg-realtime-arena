using Colyseus.Schema;

namespace RealtimeArena.Room
{
	public partial class GamePlayer : Schema
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

		[Type(5, "uint8")]
		public byte team = 0;

		[Type(6, "uint8")]
		public byte state = 0;
	}
}
