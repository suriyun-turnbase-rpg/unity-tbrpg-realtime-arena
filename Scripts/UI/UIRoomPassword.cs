using RealtimeArena.Room;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RealtimeArena.UI
{
    public class UIRoomPassword : UIBase
    {
        public Text textTitle;
        public InputField inputPassword;

        public string RoomId { get; set; }

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

        protected override void Awake()
        {
            base.Awake();
            if (inputPassword)
            {
                inputPassword.inputType = InputField.InputType.Password;
                inputPassword.contentType = InputField.ContentType.Pin;
                inputPassword.characterLimit = GameRoomConsts.MAX_PASSWORD_LENGTH;
            }
        }

        public void OnClickJoin()
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            if (inputPassword && !string.IsNullOrEmpty(inputPassword.text))
                options[GameRoomConsts.OPTION_PASSWORD] = inputPassword.text;
            RealtimeArenaManager.Instance.JoinRoom(RoomId, options);
        }
    }
}