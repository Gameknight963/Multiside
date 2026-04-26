using Photon.Realtime;
using Photon.Client;
using UnityEngine;

namespace MultiSide
{
    public class PhotonCallbacks : IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback, IInRoomCallbacks
    {
        public void OnConnected()
            => ModController.CoolLogger.Msg("Connected to Photon server");

        public void OnConnectedToMaster()
        {
            ModController.CoolLogger.Msg("Connected to master server");
        }

        public void OnDisconnected(DisconnectCause cause)
            => ModController.CoolLogger.Msg($"Disconnected from server: {cause}");

        public void OnJoinedRoom()
        {
            ModController.CoolLogger.Msg("Joined room successfully");
            foreach (KeyValuePair<int, Player> kvp in PhotonManager.Instance.client.CurrentRoom.Players)
            {
                Player player = kvp.Value;

                if (player.ActorNumber == PhotonManager.Instance.client.LocalPlayer.ActorNumber)
                    continue;

                ModController.CoolLogger.Msg($"Spawning existing player {player.NickName} ({player.ActorNumber})");

                GameObject kiriInstance = HelperFunctions.CreateKiri();
                PhotonManager.Instance.playerObjects[player.ActorNumber] = kiriInstance;
            }
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            ModController.CoolLogger.Msg($"Join random room failed: {returnCode} - {message}");
            ModController.CoolLogger.Msg("No valid room found, creating default room");

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 20,
                IsVisible = true
            };
            EnterRoomArgs enterParams = new EnterRoomArgs
            {
                RoomName = Config.DefaultRoom,
                RoomOptions = roomOptions
            };
            PhotonManager.Instance.client.OpCreateRoom(enterParams);
        }

        public void OnEvent(EventData photonEvent)
        {
            object data = photonEvent.CustomData;
            switch (photonEvent.Code)
            {
                case 0:
                    PhotonHashtable ht = (PhotonHashtable)data;
                    int actorNumber = (int)ht["actor"];
                    float[] posArray = (float[])ht["pos"];
                    float[] rotArray = (float[])ht["rot"];
                    Vector3 pos = new Vector3(posArray[0], posArray[1], posArray[2]);
                    Quaternion rot = new Quaternion(rotArray[0], rotArray[1], rotArray[2], rotArray[3]); 
                    PlayerPositionData posData = new PlayerPositionData(actorNumber, pos, rot);
                    PhotonManager.Instance.UpdatePlayer(posData);
                    break;
                case 99:
                    PhotonHashtable ht99 = (PhotonHashtable)data;
                    string channel = (string)ht99["channel"];
                    object payload = ht99["data"];
                    PhotonManager.Instance.RouteReceived(photonEvent.Sender, channel, payload);
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
            PhotonManager.Instance.playerObjects[newPlayer.ActorNumber] = kiriInstance;
            PhotonManager.Instance.FirePlayerJoined(newPlayer.ActorNumber);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            ModController.CoolLogger.Msg($"Player left room: {otherPlayer.NickName} ({otherPlayer.ActorNumber})");
            if (PhotonManager.Instance.playerObjects.TryGetValue(otherPlayer.ActorNumber, out GameObject? obj))
            {
                GameObject.Destroy(obj);
                PhotonManager.Instance.playerObjects.Remove(otherPlayer.ActorNumber);
                PhotonManager.Instance.FirePlayerLeft(otherPlayer.ActorNumber);
            }
        }

        public void OnMasterClientSwitched(Player newMasterClient)
            => ModController.CoolLogger.Msg($"Master client switched: {newMasterClient.NickName} ({newMasterClient.ActorNumber})");

        // The following callbacks are required but not used yet
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
}
