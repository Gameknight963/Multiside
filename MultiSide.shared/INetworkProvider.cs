using System;
using System.Collections.Generic;

namespace MultiSide.shared
{
    public interface INetworkProvider
    {
        bool IsConnected { get; }
        IReadOnlyList<int> ConnectedActors { get; }

        void Send(string channel, object data, bool reliable = true);
        event Action<int, string, object> OnReceived;

        event Action<int> OnPlayerJoined;
        event Action<int> OnPlayerLeft;
    }
}
