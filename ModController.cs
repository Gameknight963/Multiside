using MelonLoader;
using UnityEngine;

namespace MSZmultiplayer
{
    public class ModController : MelonMod
    {
        private GameObject kiri;
        public static MelonLogger.Instance CoolLogger;
        private const float updateIntervalMs = 100;
        private float sendTimer = 0;

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
        public override void OnFixedUpdate()
        {
            sendTimer += Time.fixedUnscaledDeltaTime;
            LoggerInstance.Msg(sendTimer);
            if (sendTimer*1000 > updateIntervalMs)
            {
                sendTimer = 0;
                PlayerPositionData myData = new PlayerPositionData
                (
                    PhotonManager.client.LocalPlayer.ActorNumber,
                    kiri.transform.position,
                    kiri.transform.rotation
                );
                PhotonManager.SendPlayerData(myData);
            }
        }
    }
}
