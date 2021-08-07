using Colyseus.Schema;

namespace RealtimeArena.Room
{
    public class LobbyRoomState : Schema
    {
        public const string ROOM_NAME = "LOBBY";
        public const string OPTION_TITLE = "title";
        public const string OPTION_PASSWORD = "password";
        public const int MAX_PASSWORD_LENGTH = 9;

        // TODO: May have player info, character level and so on.
    }
}
