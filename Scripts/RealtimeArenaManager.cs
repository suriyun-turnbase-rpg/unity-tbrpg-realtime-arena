using UnityEngine;
using Colyseus;
using RealtimeArena.Room;
using RealtimeArena.Event;
using UnityEngine.Events;

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
            onJoinLobby.Invoke();
        }

        public void OnJoinLobbyFailed(string message)
        {
            Debug.LogError($"Join Lobby Failed: {message}");
            onJoinLobbyFailed.Invoke(message);
        }
    }
}
