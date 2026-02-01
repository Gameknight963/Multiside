using Photon.Realtime;
using Photon.Client;
using System.Collections.Generic;
using UnityEngine;

namespace MSZmultiplayer
{
    public static class PhotonManager
    {
        public static RealtimeClient client;
        public static PhotonCallbacks callbacks;
        public static Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

        public static void Init()
        {
            ModController.CoolLogger.Msg("Initializing!");
            if (client != null) return;

            client = new RealtimeClient(Config.settings.Protocol);
            client.ConnectUsingSettings(Config.settings);

            callbacks = new PhotonCallbacks();
            client.AddCallbackTarget(callbacks);
        }

        public static void SendPlayerData(PlayerPositionData data)
        {
            PhotonManager.client.OpRaiseEvent(
            eventCode: 2,
            customEventContent: data,
            raiseEventArgs: new RaiseEventArgs { Receivers = ReceiverGroup.Others },
            sendOptions: new SendOptions { Reliability = false });
        }
        public static void UpdatePlayer(PlayerPositionData posData)
        {
            if (playerObjects.TryGetValue(posData.actorNumber, out GameObject playerObj))
            {
                playerObj.transform.position = posData.position;
                playerObj.transform.rotation = posData.rotation;
            }
        }
        public static void Service()
        {
            client?.Service();
        }
    }
}
