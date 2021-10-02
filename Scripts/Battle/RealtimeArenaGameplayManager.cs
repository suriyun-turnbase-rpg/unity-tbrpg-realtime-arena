using RealtimeArena.Enums;
using RealtimeArena.Message;
using RealtimeArena.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RealtimeArena.Battle
{
    public class RealtimeArenaGameplayManager : GamePlayManager
    {
        protected int loadedFormation = 0;
        protected readonly Dictionary<string, CharacterEntity> allCharacters = new Dictionary<string, CharacterEntity>();
        protected ERoomState currentState;

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
            RealtimeArenaManager.CurrentRoom.OnMessage<UpdateGameplayStateMsg>("updateGameplayState", OnUpdateGameplayState);
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
            if (currentState != (ERoomState)state.state)
            {
                currentState = (ERoomState)state.state;
                switch (currentState)
                {
                    case ERoomState.Battle:
                        CurrentWave = 0;
                        StartCoroutine(OnStartBattleRoutine());
                        break;
                }
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
                ActiveCharacter.CurrentTimeCount = 0;

            CharacterEntity activatingCharacter = null;
            var maxTime = int.MinValue;
            foreach (var character in allCharacters.Values)
            {
                if (character.Hp > 0)
                {
                    int spd = (int)character.GetTotalAttributes().spd;
                    if (spd <= 0)
                        spd = 1;
                    character.CurrentTimeCount += spd;
                    if (character.CurrentTimeCount > maxTime)
                    {
                        maxTime = character.CurrentTimeCount;
                        activatingCharacter = character;
                    }
                }
                else
                {
                    character.CurrentTimeCount = 0;
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
                else if (RealtimeArenaManager.CurrentRoom.State.players.Count == 1)
                {
                    // Another player exit the game
                    ActiveCharacter.RandomAction();
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

        private void OnUpdateGameplayState(UpdateGameplayStateMsg msg)
        {
            foreach (var character in msg.characters)
            {
                allCharacters[character.entityId].Hp = character.currentHp;
                allCharacters[character.entityId].CurrentTimeCount = character.currentTimeCount;
                var skills = character.skills;
                if (skills != null && skills.Count > 0)
                {
                    foreach (var skill in skills)
                    {
                        if (skill.index <= 0 || skill.index > allCharacters[character.entityId].Skills.Count)
                            continue;
                        var characterSkill = allCharacters[character.entityId].Skills[skill.index] as CharacterSkill;
                        characterSkill.TurnsCount = skill.turnsCount;
                        allCharacters[character.entityId].Skills[skill.index] = characterSkill;
                    }
                }
                var buffs = character.buffs;
                if (buffs != null && buffs.Count > 0)
                {
                    foreach (var buff in buffs)
                    {
                        if (!allCharacters[character.entityId].Buffs.ContainsKey(buff.id))
                            continue;
                        var characterBuff = allCharacters[character.entityId].Buffs[buff.id] as CharacterBuff;
                        characterBuff.TurnsCount = buff.turnsCount;
                        allCharacters[character.entityId].Buffs[buff.id] = characterBuff;
                    }
                }
            }
            if (!string.IsNullOrEmpty(msg.winnerSessionId))
            {
                isEnding = true;
                if (msg.winnerSessionId.Equals(RealtimeArenaManager.CurrentRoom.SessionId))
                {
                    // Show win dialog
                    uiWin.Show();
                }
                else
                {
                    // Show lose dialog
                    uiLose.Show();
                }
            }
        }

        public override void NotifyEndAction(CharacterEntity character)
        {
            if (!RealtimeArenaManager.IsManager)
                return;

            if (character != ActiveCharacter)
                return;

            string winnerSessionId = string.Empty;
            if (!CurrentTeamFormation.IsAnyCharacterAlive())
            {
                // Manager lose
                ActiveCharacter = null;
                foreach (var sessionId in RealtimeArenaManager.CurrentRoom.State.players.Keys)
                {
                    if ((string)sessionId != RealtimeArenaManager.CurrentRoom.SessionId)
                    {
                        winnerSessionId = (string)sessionId;
                        break;
                    }
                }
            }
            else if (!CurrentTeamFormation.foeFormation.IsAnyCharacterAlive())
            {
                // Manager win
                ActiveCharacter = null;
                winnerSessionId = RealtimeArenaManager.CurrentRoom.SessionId;
            }
            else
            {
                // No winner yet.
                NewTurn();
            }

            // Send characters updating to server
            var msg = new UpdateGameplayStateMsg()
            {
                winnerSessionId = winnerSessionId,
                characters = new List<UpdateCharacterEntityMsg>()
            };
            foreach (var updatingCharacter in allCharacters)
            {
                var updatingSkills = new List<UpdateCharacterSkillMsg>();
                for (int i = 0; i < updatingCharacter.Value.Skills.Count; ++i)
                {
                    updatingSkills.Add(new UpdateCharacterSkillMsg()
                    {
                        index = i,
                        turnsCount = (updatingCharacter.Value.Skills[i] as CharacterSkill).TurnsCount,
                    });
                }
                var updatingBuffs = new List<UpdateCharacterBuffMsg>();
                foreach (var buff in updatingCharacter.Value.Buffs)
                {
                    updatingBuffs.Add(new UpdateCharacterBuffMsg()
                    {
                        id = buff.Key,
                        turnsCount = (buff.Value as CharacterBuff).TurnsCount,
                    });
                }
                msg.characters.Add(new UpdateCharacterEntityMsg()
                {
                    entityId = updatingCharacter.Key,
                    currentHp = (int)updatingCharacter.Value.Hp,
                    currentTimeCount = updatingCharacter.Value.CurrentTimeCount,
                    skills = updatingSkills,
                    buffs = updatingBuffs,
                });
            }
            RealtimeArenaManager.Instance.SendUpdateGameplayState(msg);
        }

        public override void NextWave()
        {
            // Override to do nothing
        }

        public override void OnRevive()
        {
            // Override to do nothing
        }

        public override void Restart()
        {
            // Override to do nothing
        }

        public override void Giveup(UnityAction onError)
        {
            RealtimeArenaManager.CurrentRoom.Leave(true).RunSynchronously();
            GameInstance.Singleton.LoadManageScene();
        }

        public override void Revive(UnityAction onError)
        {
            // Override to do nothing
        }
    }
}
