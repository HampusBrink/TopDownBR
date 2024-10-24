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

        private void Start()
        {
            NetworkManager.TransportManager.Transport.OnClientConnectionState += Transport_OnClientConnectionState;
            NetworkManager.ServerManager.OnRemoteConnectionState += OnServerRemoteConnectionState;
        }

        private void OnServerRemoteConnectionState(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
        {
            print("Client Left!!!");
        }

        private void Transport_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            //if(obj.ConnectionState != LocalConnectionState.Stopped) return;
            print("Client Left");
            print(NetworkManager.ClientManager.Connection.Objects.Count);
            foreach (NetworkObject networkObject in NetworkManager.ClientManager.Connection.Objects)
            {
                print(networkObject.gameObject.name + "Got Destroyed");
                networkObject.RemoveOwnership();
            }
            
            SteamMatchmaking.RequestLobbyData(new CSteamID(SteamManager.Instance.CurrentLobbyID));
        }
        
        public override void OnStartServer()
        {
            ServerManager.Objects.OnPreDestroyClientObjects += Objects_OnPreDestroyClientObjects;
        }

        private void Objects_OnPreDestroyClientObjects(NetworkConnection conn)
        {
            print("YEEPERS");
            foreach (NetworkObject networkObject in conn.Objects)
                networkObject.RemoveOwnership();
        }

        public void LeaveGame()
        {
            ServerManager.StopConnection(true);
        }
    }
}
