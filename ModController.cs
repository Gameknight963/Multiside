using MelonLoader;
using UnityEngine;

namespace MSZmultiplayer
{
    public class ModController : MelonMod
    {
        private GameObject kiri;
        public static MelonLogger.Instance CoolLogger;

        public override void OnInitializeMelon()
        {
            CoolLogger = LoggerInstance;
            PhotonManager.Init();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Version 1.9 POST") return;
            PhotonManager.client.OpJoinRandomRoom();
            //HelperFunctions.CreateKiri();
        }

        public override void OnUpdate()
        {
            PhotonManager.Service();
        }
    }
}
