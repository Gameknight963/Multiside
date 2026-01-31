using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Client;
using Photon.Realtime;

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
            if (sceneName != "Version 1.9 POST") { return; }
            HelperFunctions.CreateKiri();
        }
        public override void OnUpdate()
        {
            PhotonManager.Service();
        }
    }
    public class HelperFunctions
    {
        public static GameObject CreateKiri()
        {
            if (SceneManager.GetActiveScene().name != "Version 1.9 POST") { return null; }
            GameObject kiri = GameObject.Find("Kiri");
            GameObject kiri2 = GameObject.Instantiate(kiri);
            GameObject cameraObject = kiri2.transform.Find("Zero/PLAYER Armature/Rig Root/Hips/Spine/Chest/Neck2/Neck1/Head/CameraHoldHead/playerCamera").gameObject;
            cameraObject.SetActive(false);
            return kiri2;
        }
    }
    public class PhotonManager
    {
        public static RealtimeClient client;
        public static PhotonCallbacks callbacks;
        
        public static Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
        public static void Init()
        {
            ModController.CoolLogger.Msg("Initializing!");
            if (client != null) { return; }
            client = new RealtimeClient(Config.settings.Protocol);
            client.ConnectUsingSettings(Config.settings);
            callbacks = new PhotonCallbacks();
            client.AddCallbackTarget(callbacks);
        }
        public static void Service()
        {
            client?.Service();
        }
    }
    public class PhotonCallbacks : IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback, IInRoomCallbacks
    {
        public void OnConnected()
            => ModController.CoolLogger.Msg("Connected to Photon server");

        public void OnConnectedToMaster()
        {
            ModController.CoolLogger.Msg("Connected to master server");
            PhotonManager.client.OpJoinRandomRoom();
        }

        public void OnDisconnected(DisconnectCause cause)
            => ModController.CoolLogger.Msg($"Disconnected from server: {cause}");

        public void OnJoinedRoom()
        {
            ModController.CoolLogger.Msg("Joined room successfully");
            PhotonManager.client.OpRaiseEvent(
                eventCode: 1,
                customEventContent: "Hello World",
                raiseEventArgs: new RaiseEventArgs { Receivers = ReceiverGroup.Others },
                sendOptions: new SendOptions { Reliability = true }
            );
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            ModController.CoolLogger.Msg($"Join random room failed: {returnCode} - {message}");
            ModController.CoolLogger.Msg("No valid room found, creating default room");

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 8,
                IsVisible = true
            };
            EnterRoomArgs enterParams = new EnterRoomArgs
            {
                RoomName = Config.DefaultRoom,
                RoomOptions = roomOptions
            };
            PhotonManager.client.OpCreateRoom(enterParams);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case 1:
                    object data = photonEvent.CustomData;
                    ModController.CoolLogger.Msg($"Received event: {data}");
                    break;
                default:
                    ModController.CoolLogger.Msg($"Unknown event code: {photonEvent.Code}");
                    break;
            }
        }
        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            ModController.CoolLogger.Msg($"Player entered room: {newPlayer.NickName} ({newPlayer.ActorNumber})");
            GameObject kiriInstance = HelperFunctions.CreateKiri();
            PhotonManager.playerObjects[newPlayer.ActorNumber] = kiriInstance;
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
            => ModController.CoolLogger.Msg($"Player left room: {otherPlayer.NickName} ({otherPlayer.ActorNumber})");

        public void OnMasterClientSwitched(Player newMasterClient)
            => ModController.CoolLogger.Msg($"Master client switched: {newMasterClient.NickName} ({newMasterClient.ActorNumber})");

        public void OnCreatedRoom() { }
        public void OnCreateRoomFailed(short returnCode, string message) { }
        public void OnJoinRoomFailed(short returnCode, string message) { }
        public void OnLeftRoom() { }
        public void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps) { }
        public void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged) { }
        public void OnCustomAuthenticationFailed(string debugMessage) { }
        public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }
        public void OnRegionListReceived(RegionHandler regionHandler) { }
    }


    public class Config
    {
        public static string DefaultRoom = "KoolRoom";
        public static AppSettings settings = new AppSettings
        {
            AppIdRealtime = "301df3f0-b282-4a33-b588-60e22cdf7d87",
            AppVersion = "0.1",
            FixedRegion = "us",
            Protocol = ConnectionProtocol.Udp,
            NetworkLogging = LogLevel.Info
        };
    }
}
