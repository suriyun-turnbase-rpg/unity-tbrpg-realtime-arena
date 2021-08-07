using Colyseus;
using RealtimeArena.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RealtimeArena.UI
{
    public class UIRoomListEntry : UIBase
    {
        public UIRoomList uiRoomList;
        public Text textTitle;
        public InputField inputPassword;

        public string RoomId { get; set; }
        public bool HasPassword { get; set; }

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

        public void OnClickJoin()
        {
            // Show room password UI if the room password is required
            if (HasPassword)
                uiRoomList.ShowUIRoomPassword();
            else
                Join();
        }

        private async void Join()
        {
            try
            {
                ColyseusRoom<LobbyRoomState> room = await RealtimeArenaManager.Client.JoinById<LobbyRoomState>(RoomId);
                RealtimeArenaManager.Instance.OnJoinLobby(room);
            }
            catch (System.Exception ex)
            {
                RealtimeArenaManager.Instance.OnJoinLobbyFailed(ex.Message);
            }
        }
    }
}
