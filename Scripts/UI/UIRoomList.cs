using RealtimeArena.Room;
using UnityEngine;

namespace RealtimeArena.UI
{
    public class UIRoomList : UIBase
    {
        public UIRoomListEntry entryPrefab;
        public Transform entryContainer;
        public UIRoomPassword uiRoomPassword;

        private void OnEnable()
        {
            LoadAvailableLobby();
        }

        public async void LoadAvailableLobby()
        {
            entryContainer.RemoveAllChildren();
            LoobyRoomAvailable[] rooms = await RealtimeArenaManager.Client.GetAvailableRooms<LoobyRoomAvailable>(LobbyRoomConsts.ROOM_NAME);
            for (int i = 0; i < rooms.Length; ++i)
            {
                UIRoomListEntry newRoomUI = Instantiate(entryPrefab, entryContainer);
                newRoomUI.RoomId = rooms[i].roomId;
                newRoomUI.RoomTitle = rooms[i].title;
                newRoomUI.HasPassword = rooms[i].hasPassword;
                newRoomUI.Show();
            }
        }

        public void ShowUIRoomPassword(string roomId, string roomTitle)
        {
            uiRoomPassword.RoomId = roomId;
            uiRoomPassword.RoomTitle = roomTitle;
            uiRoomPassword.Show();
        }
    }
}
