using System.Collections.Generic;

namespace RealtimeArena.Message
{
    [System.Serializable]
    public struct UpdateCharacterEntityMsg
    {
        public string entityId;
        public int currentHp;
        public int currentTimeCount;
        public List<UpdateCharacterSkillMsg> skills;
        public List<UpdateCharacterBuffMsg> buffs;
    }
}
