using RealtimeArena.Room;
using System.Threading.Tasks;
using UnityEngine;

namespace RealtimeArena.UI
{
    public class UIRoomList : UIBase
    {
        public UIRoomListEntry entryPrefab;
        public Transform entryContainer;
        public UIRoomPassword uiRoomPassword;

        private void Start()
        {
            LoadAvailableLobby(1000);
        }

        public async void LoadAvailableLobby(int milliseondsDelay)
        {
            await Task.Delay(milliseondsDelay);
            entryContainer.RemoveAllChildren();
            LoobyRoomAvailable[] rooms = await RealtimeArenaManager.Client.GetAvailableRooms<LoobyRoomAvailable>(GameRoomConsts.ROOM_NAME);
            for (int i = 0; i < rooms.Length; ++i)
            {
                UIRoomListEntry newRoomUI = Instantiate(entryPrefab, entryContainer);
                newRoomUI.uiRoomList = this;
                newRoomUI.RoomId = rooms[i].roomId;
                newRoomUI.RoomTitle = rooms[i].metadata.title;
                newRoomUI.HasPassword = rooms[i].metadata.hasPassword;
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
