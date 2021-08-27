using RealtimeArena.Enums;
using RealtimeArena.Message;
using RealtimeArena.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealtimeArena.Battle
{
    public class RealtimeArenaGameplayManager : GamePlayManager
    {
        protected int loadedFormation = 0;
        protected readonly Dictionary<string, CharacterEntity> allCharacters = new Dictionary<string, CharacterEntity>();

        protected override void Awake()
        {
            // Clear helper, online gameplay manager not allow helper
            Helper = null;
            // Set battle type to arena
            BattleType = EBattleType.Arena;
            base.Awake();
            RealtimeArenaManager.Instance.onRoomStateChange.AddListener(OnStateChange);
            RealtimeArenaManager.CurrentRoom.OnMessage<string>("updateActiveCharacter", OnUpdateActiveCharacter);
            RealtimeArenaManager.CurrentRoom.OnMessage<DoSelectedActionMsg>("doSelectedAction", OnDoSelectedAction);
        }

        protected virtual void OnDestroy()
        {
            RealtimeArenaManager.Instance.onRoomStateChange.RemoveListener(OnStateChange);
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
                    foreach (var character in teamAFormation.Characters.Values)
                    {
                        character.gameObject.AddComponent<RealtimeArenaCharacterEntityActionOverride>();
                        allCharacters.Add(character.Item.id, character as CharacterEntity);
                    }
                }
                else if (player.team == 1)
                {
                    teamBFormation.SetCharacters(result.characters.ToArray());
                    foreach (var character in teamBFormation.Characters.Values)
                    {
                        character.gameObject.AddComponent<RealtimeArenaCharacterEntityActionOverride>();
                        allCharacters.Add(character.Item.id, character as CharacterEntity);
                    }
                }
                loadedFormation++;
                if (loadedFormation >= 2)
                {
                    // Tell the server that the this client is enter the game
                    RealtimeArenaManager.Instance.SetPlayerEnterGameState();
                }
            });
        }

        private void OnStateChange(GameRoomState state, bool isFirstState)
        {
            switch ((ERoomState)state.state)
            {
                case ERoomState.Battle:
                    CurrentWave = 0;
                    StartCoroutine(OnStartBattleRoutine());
                    break;
            }
        }

        private IEnumerator OnStartBattleRoutine()
        {
            yield return null;
            CurrentTeamFormation.MoveCharactersToFormation(false);
            CurrentTeamFormation.foeFormation.MoveCharactersToFormation(false);
            yield return new WaitForSeconds(moveToNextWaveDelay);
            NewTurn();
        }

        protected override void Update()
        {
            base.Update();
            // Fix timescale to 1
            Time.timeScale = 1;
        }

        public override void NewTurn()
        {
            if (!RealtimeArenaManager.IsManager)
                return;

            if (ActiveCharacter != null)
                ActiveCharacter.currentTimeCount = 0;

            CharacterEntity activatingCharacter = null;
            var maxTime = int.MinValue;
            foreach (var character in allCharacters.Values)
            {
                if (character.Hp > 0)
                {
                    int spd = (int)character.GetTotalAttributes().spd;
                    if (spd <= 0)
                        spd = 1;
                    character.currentTimeCount += spd;
                    if (character.currentTimeCount > maxTime)
                    {
                        maxTime = character.currentTimeCount;
                        activatingCharacter = character;
                    }
                }
                else
                {
                    character.currentTimeCount = 0;
                }
            }
            // Broadcast activate character
            RealtimeArenaManager.Instance.SendUpdateActiveCharacter(activatingCharacter.Item.Id);
        }

        private void OnUpdateActiveCharacter(string id)
        {
            ActiveCharacter = allCharacters[id];
            ActiveCharacter.DecreaseBuffsTurn();
            ActiveCharacter.DecreaseSkillsTurn();
            ActiveCharacter.ResetStates();
            if (ActiveCharacter.Hp > 0 &&
                !ActiveCharacter.IsStun)
            {
                if (ActiveCharacter.IsPlayerCharacter)
                {
                    if (IsAutoPlay)
                        ActiveCharacter.RandomAction();
                    else
                        uiCharacterActionManager.Show();
                }
            }
            else
            {
                ActiveCharacter.NotifyEndAction();
            }
        }

        private void OnDoSelectedAction(DoSelectedActionMsg msg)
        {
            CharacterEntity character = allCharacters[msg.entityId];
            CharacterEntity target = null;
            if (!string.IsNullOrEmpty(msg.targetEntityId))
                target = allCharacters[msg.targetEntityId];
            character.Action = msg.action;
            character.ActionTarget = target;
            if (character.Action == CharacterEntity.ACTION_ATTACK)
                character.DoAttackAction(msg.seed);
            else
                character.DoSkillAction(msg.seed);
        }

        public override void NotifyEndAction(CharacterEntity character)
        {
            if (!RealtimeArenaManager.IsManager)
                return;

            if (character != ActiveCharacter)
                return;

            if (!CurrentTeamFormation.IsAnyCharacterAlive())
            {
                // Manager lose
                ActiveCharacter = null;
                // TODO: send game result to the server
            }
            else if (!CurrentTeamFormation.foeFormation.IsAnyCharacterAlive())
            {
                // Manager win
                ActiveCharacter = null;
                // TODO: send game result to the server
            }
            else
            {
                // No winner yet.
                NewTurn();
            }
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
