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

        public static void Service()
        {
            client?.Service();
        }
    }
}
