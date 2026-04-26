using MelonLoader;
using UnityEngine;

namespace MultiSide
{
    public class ModController : MelonMod
    {
        private GameObject kiri;
        public static MelonLogger.Instance CoolLogger;
        private const float _updateInterval = 0.1f;
        // set the send timer to updateinterval so it sends right away
        private float _sendTimer = _updateInterval;

        public override void OnInitializeMelon()
        {
            CoolLogger = LoggerInstance;
            PhotonManager.Init();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Version 1.9 POST") return;
            kiri = GameObject.Find("Kiri");
            PhotonManager.client.OpJoinRandomRoom();
        }

        public override void OnUpdate()
        {
            PhotonManager.Service();
        }
        public override void OnFixedUpdate()
        {
            if (PhotonManager.client == null) LoggerInstance.Msg("client null");
            if (PhotonManager.client?.LocalPlayer == null) LoggerInstance.Msg("LocalPlayer null");

            if (PhotonManager.client == null || PhotonManager.client.LocalPlayer == null || kiri == null)
                return;
            _sendTimer += Time.fixedUnscaledDeltaTime;
            if (_sendTimer > _updateInterval)
            {
                _sendTimer = 0;
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
