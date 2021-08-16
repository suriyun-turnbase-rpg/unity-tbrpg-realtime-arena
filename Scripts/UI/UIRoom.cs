using RealtimeArena.Room;
using UnityEngine;
using UnityEngine.UI;

namespace RealtimeArena.UI
{
    public class UIRoom : UIBase
    {
        public UIRoomPlayer playerPrefab;
        public Transform playerContainer;
        public Text textTitle;
        public bool IsReady { get; set; }

        private string _roomTitle;
        public string RoomTitle
        {
            get { return _roomTitle; }
            set
            {
                _roomTitle = value;
                if (textTitle)
                    textTitle.text = value;
            }
        }

        private void OnEnable()
        {
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

        private void OnStateChange(LobbyRoomState state, bool isFirstState)
        {
            UpdateRoomState(state);
        }

        private void OnLeave(int code)
        {

        }

        private void UpdateRoomState(LobbyRoomState state)
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
