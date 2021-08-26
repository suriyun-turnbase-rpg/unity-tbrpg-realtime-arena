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
        public static ColyseusRoom<GameRoomState> CurrentRoom { get; set; }

        public string serverAddress = "ws://localhost:2567";
        public string battleScene = "OnlineBattleScene";
        public UnityEvent onJoinLobby = new UnityEvent();
        public StringEvent onJoinLobbyFailed = new StringEvent();
        public RoomErrorEvent onRoomError = new RoomErrorEvent();
        public RoomLeaveEvent onRoomLeave = new RoomLeaveEvent();
        public LobbyEvent.LobbyStateChangeEvent onRoomStateChange = new LobbyEvent.LobbyStateChangeEvent();

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

        public void OnJoinLobby(ColyseusRoom<GameRoomState> room)
        {
            CurrentRoom = room;
            CurrentRoom.OnError += CurrentRoom_OnError;
            CurrentRoom.OnStateChange += CurrentRoom_OnStateChange;
            CurrentRoom.OnLeave += CurrentRoom_OnLeave;
            onJoinLobby.Invoke();
        }

        private void CurrentRoom_OnError(int code, string message)
        {
            onRoomError.Invoke(code, message);
        }

        private void CurrentRoom_OnStateChange(GameRoomState state, bool isFirstState)
        {
            onRoomStateChange.Invoke(state, isFirstState);
        }

        private void CurrentRoom_OnLeave(int code)
        {
            onRoomLeave.Invoke(code);
        }

        public void OnJoinLobbyFailed(string message)
        {
            Debug.LogError($"Join Lobby Failed: {message}");
            onJoinLobbyFailed.Invoke(message);
        }

        public void LoadBattleScene(bool loadIfNotLoaded = false)
        {
            GameInstance.Singleton.onLoadSceneFinish.RemoveListener(OnLoadBattleSceneFinish);
            GameInstance.Singleton.onLoadSceneFinish.AddListener(OnLoadBattleSceneFinish);
            GameInstance.Singleton.LoadSceneIfNotLoaded(battleScene, loadIfNotLoaded);
        }

        private void OnLoadBattleSceneFinish(string sceneName, float progress)
        {
            SetPlayerEnterGameState();
            GameInstance.Singleton.onLoadSceneFinish.RemoveListener(OnLoadBattleSceneFinish);
        }

        public async void TogglePlayerReadyState()
        {
            await CurrentRoom.Send("ready");
        }

        public async void SetPlayerEnterGameState()
        {
            await CurrentRoom.Send("enterGame");
        }
    }
}
