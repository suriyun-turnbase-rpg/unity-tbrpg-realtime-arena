using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealtimeArena.Message
{
    [System.Serializable]
    public struct DoSelectedActionMsg
    {
        public string entityId;
        public string targetEntityId;
        public int action;
        public int seed;
    }
}
