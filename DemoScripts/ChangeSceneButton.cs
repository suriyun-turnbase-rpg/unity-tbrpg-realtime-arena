using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealtimeArena.DemoScripts
{
    public class ChangeSceneButton : MonoBehaviour
    {
        public string sceneName;
        public void OnClickChangeScene()
        {
            GameInstance.Singleton.LoadSceneIfNotLoaded(sceneName);
        }
    }
}
