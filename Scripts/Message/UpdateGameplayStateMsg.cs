using System.Collections.Generic;

namespace RealtimeArena.Message
{
    [System.Serializable]
    public struct UpdateGameplayStateMsg
    {
        public string winnerSessionId;
        public List<UpdateCharacterEntityMsg> characters;
    }
}
