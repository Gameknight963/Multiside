# Documentation (for mod developers)

## Setup

Add a reference to `Multiside.shared.dll` in your project.

The network provider is registered at runtime by Multiside. Always null-check `NetworkRegistry.Provider` before using it, if no backend is loaded it will be null.

---

## NetworkRegistry

The entry point. Holds the active network provider.

```csharp
public static class NetworkRegistry
{
    public static INetworkProvider? Provider { get; }
}
```

Check `Provider != null` before calling anything. If the backend mod isn't loaded, `Provider` stays null and your mod should skip all network behavior gracefully.

---

## INetworkProvider

### Properties

| Property | Type | Description |
|---|---|---|
| `IsConnected` | `bool` | Whether the client is currently connected to a room. |
| `ConnectedActors` | `IReadOnlyList<int>` | Actor numbers of other players currently in the room. Does not include the local player. |

### Methods

| Method | Description |
|---|---|
| `Send(string channel, object data, bool reliable = true)` | Sends data to all other players in the room. |
| `SendTo(int actor, string channel, object data, bool reliable = true)` | Sends data to a specific player by actor number. |

### Events

| Event | Signature | Description |
|---|---|---|
| `OnReceived` | `Action<int, string, object>` | Fired when data is received. Parameters are sender actor number, channel, and data. |
| `OnPlayerJoined` | `Action<int>` | Fired when a player joins the room. Parameter is their actor number. |
| `OnPlayerLeft` | `Action<int>` | Fired when a player leaves the room. Parameter is their actor number. |

---

## Usage

### Sending data

```csharp
NetworkRegistry.Provider?.Send("mymod.myevent", myData);
NetworkRegistry.Provider?.SendTo(actor, "mymod.myevent", myData);
```

### Receiving data

```csharp
if (NetworkRegistry.Provider != null)
{
    NetworkRegistry.Provider.OnReceived += (actor, channel, data) =>
    {
        if (channel != "mymod.myevent") return;
        // handle data
    };
}
```

### Reacting to players joining and leaving

```csharp
NetworkRegistry.Provider.OnPlayerJoined += actor => { };
NetworkRegistry.Provider.OnPlayerLeft += actor => { };
```

Use `ConnectedActors` for players already in the room when your mod loads, as `OnPlayerJoined` will not fire for them retroactively.

---

## Channels

Channels are plain strings used to route messages to the right mod. Use the format `"modname.eventname"` to avoid collisions with other mods.

---

## Serialization

Photon natively serializes primitive types and arrays of primitive types. These can be passed directly as `data`:

- `string`, `string[]`
- `int`, `float`, `bool` and their array variants
- `PhotonHashtable` for structured data with mixed types

Custom classes cannot be serialized directly. Use `PhotonHashtable` with primitive values instead, or register a custom Photon type serializer.
