using UnityEngine;
using Colyseus;
using RealtimeArena.Room;
using RealtimeArena.Event;
using UnityEngine.Events;
using LobbyEvent = RealtimeArena.Event.Lobby;

namespace RealtimeArena
{
    public class RealtimeArenaManager : MonoBehaviour
    {
        public static RealtimeArenaManager Instance { get; private set; }
        public static ColyseusClient Client { get; private set; }
        public static ColyseusRoom<LobbyRoomState> CurrentLobby { get; set; }

        public string serverAddress = "ws://localhost:2567";
        public UnityEvent onJoinLobby = new UnityEvent();
        public StringEvent onJoinLobbyFailed = new StringEvent();
        public RoomErrorEvent onLobbyError = new RoomErrorEvent();
        public RoomLeaveEvent onLobbyLeave = new RoomLeaveEvent();
        public LobbyEvent.LobbyStateChangeEvent onLobbyStateChange = new LobbyEvent.LobbyStateChangeEvent();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Client = new ColyseusClient(serverAddress);
        }

        public void OnJoinLobby(ColyseusRoom<LobbyRoomState> room)
        {
            CurrentLobby = room;
            CurrentLobby.OnError += CurrentLobby_OnError;
            CurrentLobby.OnStateChange += CurrentLobby_OnStateChange;
            CurrentLobby.OnLeave += CurrentLobby_OnLeave;
            onJoinLobby.Invoke();
        }

        private void CurrentLobby_OnError(int code, string message)
        {
            onLobbyError.Invoke(code, message);
        }

        private void CurrentLobby_OnStateChange(LobbyRoomState state, bool isFirstState)
        {
            onLobbyStateChange.Invoke(state, isFirstState);
        }

        private void CurrentLobby_OnLeave(int code)
        {
            onLobbyLeave.Invoke(code);
        }

        public void OnJoinLobbyFailed(string message)
        {
            Debug.LogError($"Join Lobby Failed: {message}");
            onJoinLobbyFailed.Invoke(message);
        }
    }
}
