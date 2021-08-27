using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealtimeArena.Battle
{
    public class RealtimeArenaGameplayManager : GamePlayManager
    {

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
        }

        protected override void SetupTeamBFormation()
        {
            teamBFormation.ClearCharacters();
            teamBFormation.foeFormation = teamAFormation;
        }

        protected override void Start()
        {
            // Tell the server that the this client is enter the game
            RealtimeArenaManager.Instance.SetPlayerEnterGameState();
        }

        protected override void Update()
        {
            base.Update();
            // Fix timescale to 1
            Time.timeScale = 1;
        }

        public override void OnRevive()
        {
            // Override to do nothing on revive
        }

        public override int CountDeadCharacters()
        {
            return CurrentTeamFormation != null ? CurrentTeamFormation.CountDeadCharacters() : 0;
        }
    }
}
