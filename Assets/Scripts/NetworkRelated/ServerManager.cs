using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using LiteNetLib;
using NetworkRelated.Steam;
using Steamworks;
using UnityEngine;

namespace NetworkRelated
{
    public class ServerManager : NetworkBehaviour
    {
        EventBasedNetListener eventListener = new ();

        private void Start()
        {
            NetworkManager.TransportManager.Transport.OnClientConnectionState += Transport_OnClientConnectionState;
            ServerManager.OnRemoteConnectionState += ServerManagerOnOnServerConnectionState;
            
        }

        private void ClientManagerOnOnClientConnectionState(ClientConnectionStateArgs obj)
        {
            
        }

        private void EventListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            Debug.Log("hejsan");
        }

        private void ServerManagerOnOnServerConnectionState(NetworkConnection networkConnection, RemoteConnectionStateArgs remoteConnectionStateArgs)
        {
            print("Client Left");
            SteamMatchmaking.RequestLobbyData(new CSteamID(SteamManager.Instance.CurrentLobbyID));
        }

        private void Transport_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            print("Client Left");
            foreach (NetworkObject networkObject in NetworkManager.ClientManager.Connection.Objects)
                networkObject.RemoveOwnership();
            
            SteamMatchmaking.RequestLobbyData(new CSteamID(SteamManager.Instance.CurrentLobbyID));
        }

        public void LeaveGame()
        {
            ServerManager.StopConnection(true);
        }
    }
}
