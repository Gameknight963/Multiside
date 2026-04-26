using MultiSide.shared;
using Photon.Client;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

namespace MultiSide
{
    public class PhotonManager : INetworkProvider
    {
        public static PhotonManager Instance { get; private set; } = new();

        public RealtimeClient client = new(Config.settings.Protocol);
        public PhotonCallbacks callbacks = new();
        public Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

        public bool IsConnected => client?.IsConnected ?? false;
        public IReadOnlyList<int> ConnectedActors => playerObjects.Keys.ToList();

        public event Action<int, string, object>? OnReceived;
        public event Action<int>? OnPlayerJoined;
        public event Action<int>? OnPlayerLeft;

        public void Init()
        {
            ModController.CoolLogger.Msg("Initializing!");
            client.ConnectUsingSettings(Config.settings);
            client.AddCallbackTarget(callbacks);
            NetworkRegistry.Register(this);
        }

        public void SendPlayerData(PlayerPositionData data)
        {
            float[] posArray = new float[3] { data.position.x, data.position.y, data.position.z };
            float[] rotArray = new float[4] { data.rotation.x, data.rotation.y, data.rotation.z, data.rotation.w };
            PhotonHashtable ht = new PhotonHashtable
        {
            { "actor", data.actorNumber },
            { "pos", posArray },
            { "rot", rotArray }
        };
            client.OpRaiseEvent(
                eventCode: 0,
                customEventContent: ht,
                raiseEventArgs: new RaiseEventArgs { Receivers = ReceiverGroup.Others },
                sendOptions: new SendOptions { Reliability = false });
        }

        public void Send(string channel, object data, bool reliable = true)
        {
            PhotonHashtable ht = new PhotonHashtable
        {
            { "channel", channel },
            { "data", data }
        };
            client.OpRaiseEvent(
                eventCode: 99,
                customEventContent: ht,
                raiseEventArgs: new RaiseEventArgs { Receivers = ReceiverGroup.Others },
                sendOptions: new SendOptions { Reliability = reliable });
        }

        public void UpdatePlayer(PlayerPositionData posData)
        {
            if (playerObjects.TryGetValue(posData.actorNumber, out GameObject? playerObj))
            {
                playerObj.transform.position = posData.position;
                playerObj.transform.rotation = posData.rotation;
            }
        }

        public void RouteReceived(int actor, string channel, object data)
            => OnReceived?.Invoke(actor, channel, data);

        public void FirePlayerJoined(int actor) => OnPlayerJoined?.Invoke(actor);
        public void FirePlayerLeft(int actor) => OnPlayerLeft?.Invoke(actor);

        public void Service() => client?.Service();
    }
}
