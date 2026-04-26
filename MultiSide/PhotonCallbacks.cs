using Photon.Realtime;
using Photon.Client;
using System.Collections.Generic;
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
            foreach (KeyValuePair<int, Player> kvp in PhotonManager.client.CurrentRoom.Players)
            {
                Player player = kvp.Value;

                if (player.ActorNumber == PhotonManager.client.LocalPlayer.ActorNumber)
                    continue;

                ModController.CoolLogger.Msg($"Spawning existing player {player.NickName} ({player.ActorNumber})");

                GameObject kiriInstance = HelperFunctions.CreateKiri();
                PhotonManager.playerObjects[player.ActorNumber] = kiriInstance;
            }
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
            object data = photonEvent.CustomData;
            switch (photonEvent.Code)
            {
                case 1:
                    ModController.CoolLogger.Msg($"Received event: {data}");
                    break;
                case 2:
                    PhotonHashtable ht = (PhotonHashtable)data;
                    int actorNumber = (int)ht["actor"];
                    float[] posArray = (float[])ht["pos"];
                    float[] rotArray = (float[])ht["rot"];
                    Vector3 pos = new Vector3(posArray[0], posArray[1], posArray[2]);
                    Quaternion rot = new Quaternion(rotArray[0], rotArray[1], rotArray[2], rotArray[3]); 
                    PlayerPositionData posData = new PlayerPositionData(actorNumber, pos, rot);
                    PhotonManager.UpdatePlayer(posData);
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
        {
            ModController.CoolLogger.Msg($"Player left room: {otherPlayer.NickName} ({otherPlayer.ActorNumber})");
            if (PhotonManager.playerObjects.TryGetValue(otherPlayer.ActorNumber, out GameObject obj))
            {
                GameObject.Destroy(obj);
                PhotonManager.playerObjects.Remove(otherPlayer.ActorNumber);
            }
            else
            {
                ModController.CoolLogger.Error($"Attempted to delete player {otherPlayer.ActorNumber}, but could not find dictionary instance");
                foreach (var kvp in PhotonManager.playerObjects)
                {
                    ModController.CoolLogger.Msg($"Player {kvp.Key} -> GameObject: {(kvp.Value != null ? kvp.Value.name : "null")}");
                }
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
