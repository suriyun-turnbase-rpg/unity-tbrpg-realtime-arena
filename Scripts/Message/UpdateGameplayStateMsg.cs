using System.Collections.Generic;

namespace RealtimeArena.Message
{
    [System.Serializable]
    public struct UpdateGameplayStateMsg
    {
        public string winnerSessionId;
        public string loserSessionId;
        public List<UpdateCharacterEntityMsg> characters;
    }
}
