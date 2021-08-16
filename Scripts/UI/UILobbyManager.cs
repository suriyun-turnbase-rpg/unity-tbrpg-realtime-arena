using UnityEngine;

namespace RealtimeArena.UI
{
    public class UILobbyManager : MonoBehaviour
    {
        public UIRoomList uiRoomList;
        public UIRoom uiRoom;

        private void Start()
        {
            uiRoom.Hide();
            uiRoomList.Show();
            RealtimeArenaManager.Instance.onJoinLobby.AddListener(OnJoin);
            RealtimeArenaManager.Instance.onLobbyLeave.AddListener(OnLeave);
        }

        private void OnDestroy()
        {
            RealtimeArenaManager.Instance.onJoinLobby.RemoveListener(OnJoin);
            RealtimeArenaManager.Instance.onLobbyLeave.RemoveListener(OnLeave);
        }

        private void OnJoin()
        {
            uiRoomList.Hide();
            uiRoom.Show();
        }

        private void OnLeave(int code)
        {
            uiRoom.Hide();
            uiRoomList.Show();
        }
    }
}
