using Colyseus;
using RealtimeArena.Room;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RealtimeArena.UI
{
    public class UIRoomCreate : UIBase
    {
        public InputField inputTitle;
        public InputField inputPassword;

        protected override void Awake()
        {
            base.Awake();
            if (inputTitle)
            {
                inputTitle.inputType = InputField.InputType.Standard;
                inputTitle.contentType = InputField.ContentType.Name;
            }
            if (inputPassword)
            {
                inputPassword.inputType = InputField.InputType.Password;
                inputPassword.contentType = InputField.ContentType.Pin;
                inputPassword.characterLimit = GameRoomConsts.MAX_PASSWORD_LENGTH;
            }
        }

        public void OnClickCreate()
        {
            Create();
        }

        private async void Create()
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            if (inputTitle && !string.IsNullOrEmpty(inputTitle.text))
                options[GameRoomConsts.OPTION_TITLE] = inputTitle.text;
            if (inputPassword && !string.IsNullOrEmpty(inputPassword.text))
                options[GameRoomConsts.OPTION_PASSWORD] = inputPassword.text;

            try
            {
                ColyseusRoom<GameRoomState> room = await RealtimeArenaManager.Client.Create<GameRoomState>(GameRoomConsts.ROOM_NAME, options);
                RealtimeArenaManager.Instance.OnJoinLobby(room);
            }
            catch (System.Exception ex)
            {
                RealtimeArenaManager.Instance.OnJoinLobbyFailed(ex.Message);
            }
        }
    }
}
