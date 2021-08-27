using RealtimeArena.Room;
using UnityEngine;

namespace RealtimeArena.Battle
{
    public class RealtimeArenaGameplayManager : GamePlayManager
    {
        protected int loadedFormation = 0;

        protected override void Awake()
        {
            // Clear helper, online gameplay manager not allow helper
            Helper = null;
            // Set battle type to arena
            BattleType = EBattleType.Arena;
            base.Awake();
        }

        protected override void SetupTeamAFormation()
        {
            teamAFormation.ClearCharacters();
            teamAFormation.foeFormation = teamBFormation;
            if (RealtimeArenaManager.CurrentRoom.State.players[RealtimeArenaManager.CurrentRoom.SessionId].team == 0)
                CurrentTeamFormation = teamAFormation;
        }

        protected override void SetupTeamBFormation()
        {
            teamBFormation.ClearCharacters();
            teamBFormation.foeFormation = teamAFormation;
            if (RealtimeArenaManager.CurrentRoom.State.players[RealtimeArenaManager.CurrentRoom.SessionId].team == 1)
                CurrentTeamFormation = teamBFormation;
        }

        protected override void Start()
        {
            foreach (var sessionId in RealtimeArenaManager.CurrentRoom.State.players.Keys)
            {
                GetArenaFormationCharactersAndEquipmentsForPlayer((string)sessionId, RealtimeArenaManager.CurrentRoom.State.players[(string)sessionId]);
            }
        }

        protected void GetArenaFormationCharactersAndEquipmentsForPlayer(string sessionId, GamePlayer player)
        {
            GameInstance.GameService.GetArenaFormationCharactersAndEquipments(player.id, (result) =>
            {
                GameInstance.Singleton.OnGameServiceGetFormationCharactersAndEquipments(result);
                if (player.team == 0)
                {
                    teamAFormation.SetCharacters(result.characters.ToArray());
                }
                else if (player.team == 1)
                {
                    teamBFormation.SetCharacters(result.characters.ToArray());
                }
                loadedFormation++;
                if (loadedFormation >= 2)
                {
                    // Tell the server that the this client is enter the game
                    RealtimeArenaManager.Instance.SetPlayerEnterGameState();
                }
            });
        }

        protected override void Update()
        {
            base.Update();
            // Fix timescale to 1
            Time.timeScale = 1;
        }

        public override void NextWave()
        {
            // Override to do nothing
        }

        public override void OnRevive()
        {
            // Override to do nothing on revive
        }
    }
}
