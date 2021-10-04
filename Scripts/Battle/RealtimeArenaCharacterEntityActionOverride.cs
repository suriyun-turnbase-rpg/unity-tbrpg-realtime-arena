using UnityEngine;

namespace RealtimeArena.Battle
{
    public class RealtimeArenaCharacterEntityActionOverride : MonoBehaviour, ICharacterEntityActionOverrider
    {
        public CharacterEntity CharacterEntity { get; private set; }

        private void Awake()
        {
            CharacterEntity = GetComponent<CharacterEntity>();
        }

        public void DoSelectedAction(int seed)
        {
            string entityId = CharacterEntity.Item.Id;
            string targetEntityId = CharacterEntity.ActionTarget != null ? CharacterEntity.ActionTarget.Item.Id : string.Empty;
            int action = CharacterEntity.Action;
            // Send do selected action to server
            RealtimeArenaManager.Instance.SendDoSelectedAction(entityId, targetEntityId, action, seed);
        }
    }
}
