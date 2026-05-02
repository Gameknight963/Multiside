using UnityEngine;

namespace MultiSide.shared
{
    public interface INetworkProvider
    {
        bool IsConnected { get; }
        IReadOnlyList<int> ConnectedActors { get; }

        void Send(string channel, object data, bool reliable = true);
        void SendTo(int actor, string channel, object data, bool reliable = true);
        event Action<int, string, object> OnReceived;

        event Action<int> OnPlayerJoined;
        event Action<int> OnPlayerLeft;

        GameObject? GetPlayerObject(int actor);

        IReadOnlyDictionary<int, GameObject> PlayerObjects { get; }

        int LocalActorNumber { get; }
        GameObject? LocalPlayerObject { get; }

        bool IsInRoom { get; }
        event Action OnRoomJoined;

        bool IsMasterClient { get; }
    }
}