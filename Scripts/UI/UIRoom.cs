using RealtimeArena.Room;
using RealtimeArena.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RealtimeArena.UI
{
    public class UIRoom : UIBase
    {
        public UIRoomPlayer playerPrefab;
        public Transform playerContainer;
        public Text textCountDown;
        public bool IsStarting { get; private set; }
        public ERoomState CurrentRoomState { get; private set; } = ERoomState.WaitPlayersToReady;

        private Coroutine countDownCoroutine = null;

        private void OnEnable()
        {
            if (textCountDown)
                textCountDown.text = string.Empty;
            RealtimeArenaManager.Instance.onLobbyError.AddListener(OnError);
            RealtimeArenaManager.Instance.onLobbyStateChange.AddListener(OnStateChange);
            RealtimeArenaManager.Instance.onLobbyLeave.AddListener(OnLeave);
            UpdateRoomState(RealtimeArenaManager.CurrentLobby.State);
        }

        private void OnDisable()
        {
            RealtimeArenaManager.Instance.onLobbyError.RemoveListener(OnError);
            RealtimeArenaManager.Instance.onLobbyStateChange.RemoveListener(OnStateChange);
            RealtimeArenaManager.Instance.onLobbyLeave.RemoveListener(OnLeave);
        }

        private void OnError(int code, string message)
        {

        }

        private void OnStateChange(GameRoomState state, bool isFirstState)
        {
            UpdateRoomState(state);
            if (state.state != (byte)CurrentRoomState)
            {
                CurrentRoomState = (ERoomState)state.state;
                IsStarting = state.state >= (byte)ERoomState.CountDownToStartGame;
                if (countDownCoroutine != null)
                {
                    StopCoroutine(countDownCoroutine);
                    countDownCoroutine = null;
                }
                if (textCountDown)
                    textCountDown.text = string.Empty;
                if (IsStarting)
                    countDownCoroutine = StartCoroutine(CountDownRoutine());
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
            foreach (var playerKey in state.players.Keys)
            {
                UIRoomPlayer newRoomUI = Instantiate(playerPrefab, playerContainer);
                newRoomUI.Player = state.players[(string)playerKey];
                newRoomUI.Show();
            }
        }

        public void OnClickReady()
        {
            OnClickReadyRoutine();
        }

        private async void OnClickReadyRoutine()
        {
            // NOTE: If 2 players click ready it will start game.
            await RealtimeArenaManager.CurrentLobby.Send("ready");
        }
    }
}
