using RealtimeArena.Room;
using RealtimeArena.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RealtimeArena.UI
{
    public class UIRoom : UIBase
    {
        public UIRoomPlayer playerPrefab;
        public Transform playerContainer;
        public Text textCountDown;

        private Coroutine countDownCoroutine = null;
        private readonly Dictionary<string, UIRoomPlayer> uiRoomPlayers = new Dictionary<string, UIRoomPlayer>();

        private void OnEnable()
        {
            if (textCountDown)
                textCountDown.text = string.Empty;
            RealtimeArenaManager.Instance.onRoomError.AddListener(OnError);
            RealtimeArenaManager.Instance.onRoomStateChange.AddListener(OnStateChange);
            RealtimeArenaManager.Instance.onLeaveRoom.AddListener(OnLeave);
            UpdateRoomState(RealtimeArenaManager.CurrentRoom.State);
        }

        private void OnDisable()
        {
            RealtimeArenaManager.Instance.onRoomError.RemoveListener(OnError);
            RealtimeArenaManager.Instance.onRoomStateChange.RemoveListener(OnStateChange);
            RealtimeArenaManager.Instance.onLeaveRoom.RemoveListener(OnLeave);
        }

        private void OnError(int code, string message)
        {

        }

        private void OnStateChange(GameRoomState state, bool isFirstState)
        {
            UpdateRoomState(state);
            if (textCountDown)
                textCountDown.text = string.Empty;
            if (countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
                countDownCoroutine = null;
            }
            switch ((ERoomState)state.state)
            {
                case ERoomState.CountDownToStartGame:
                    // Count down before enter game
                    countDownCoroutine = StartCoroutine(CountDownRoutine());
                    break;
                case ERoomState.WaitPlayersToEnterGame:
                    // Load battle scene to enter game
                    RealtimeArenaManager.Instance.LoadBattleScene();
                    break;
            }
        }

        private IEnumerator CountDownRoutine()
        {
            int countDown = GameRoomConsts.ENTER_GAME_COUNT_DOWN;
            do
            {
                if (textCountDown)
                    textCountDown.text = countDown.ToString();
                yield return new WaitForSeconds(1);
                countDown--;
            }
            while (countDown > 0);
            if (textCountDown)
                textCountDown.text = string.Empty;
        }

        private void OnLeave(int code)
        {

        }

        private void UpdateRoomState(GameRoomState state)
        {
            playerContainer.RemoveAllChildren();
            uiRoomPlayers.Clear();
            foreach (var playerKey in state.players.Keys)
            {
                UIRoomPlayer newRoomUI = Instantiate(playerPrefab, playerContainer);
                newRoomUI.Player = state.players[(string)playerKey];
                newRoomUI.Show();
                uiRoomPlayers[(string)playerKey] = newRoomUI;
            }
        }

        public void OnClickReady()
        {
            RealtimeArenaManager.Instance.TogglePlayerReadyState();
        }

        public void OnClickLeave()
        {
            RealtimeArenaManager.Instance.LeaveFromTheRoom();
        }
    }
}
