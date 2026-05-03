using MelonLoader;
using MultiSide;
using UnityEngine;
[assembly: MelonInfo(typeof(ModController), "Multiside", "1.0.2", "gameknight963")]

namespace MultiSide
{
    public class ModController : MelonMod
    {
        public static GameObject? Kiri { get; private set; }
        public static MelonLogger.Instance CoolLogger { get; private set; } = null!;
        private const float _updateInterval = 0.1f;
        // set the send timer to updateinterval so it sends right away
        private float _sendTimer = _updateInterval;

        public override void OnInitializeMelon()
        {
            CoolLogger = LoggerInstance;
            PhotonManager.Instance.Init();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Version 1.9 POST") return;
            Kiri = GameObject.Find("Kiri");
            PhotonManager.Instance.client.OpJoinRandomRoom();
        }

        public override void OnUpdate()
        {
            PhotonManager.Instance.Service();
            PhotonManager.Instance.UpdatePlayerTransforms();
        }
        public override void OnFixedUpdate()
        {
            if (PhotonManager.Instance.client == null) LoggerInstance.Msg("client null");
            if (PhotonManager.Instance.client?.LocalPlayer == null) LoggerInstance.Msg("LocalPlayer null");

            if (PhotonManager.Instance.client == null || PhotonManager.Instance.client.LocalPlayer == null || Kiri == null)
                return;
            _sendTimer += Time.fixedUnscaledDeltaTime;
            if (_sendTimer > _updateInterval)
            {
                _sendTimer = 0;
                PlayerPositionData myData = new PlayerPositionData
                (
                    PhotonManager.Instance.client.LocalPlayer.ActorNumber,
                    Kiri.transform.position,
                    Kiri.transform.rotation
                );
                PhotonManager.Instance.SendPlayerData(myData);
            }
        }
    }
}