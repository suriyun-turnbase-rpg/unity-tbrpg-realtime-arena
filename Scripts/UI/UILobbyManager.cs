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
            RealtimeArenaManager.Instance.onJoinRoom.AddListener(OnJoin);
            RealtimeArenaManager.Instance.onLeaveRoom.AddListener(OnLeave);
        }

        private void OnDestroy()
        {
            RealtimeArenaManager.Instance.onJoinRoom.RemoveListener(OnJoin);
            RealtimeArenaManager.Instance.onLeaveRoom.RemoveListener(OnLeave);
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
