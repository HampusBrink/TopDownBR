using System;
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
        public static ServerManager Instance;

        public event Action ConnectedToServer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            NetworkManager.ServerManager.Objects.OnPreDestroyClientObjects += ObjectsOnOnPreDestroyClientObjects;
            NetworkManager.ClientManager.OnClientTimeOut += ClientManagerOnOnClientTimeOut;
            NetworkManager.TransportManager.Transport.OnClientConnectionState += Transport_OnClientConnectionState;
            NetworkManager.ServerManager.OnRemoteConnectionState += OnServerRemoteConnectionState;

            NetworkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
            NetworkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
        }

        private void OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Started)
            {
                ConnectedToServer?.Invoke();
                print("Connected To Master");
            }
        }

        private void OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Starting)
            {
                ConnectedToServer?.Invoke();
                print("Connected To Master");
            }
        }

        private void ClientManagerOnOnClientTimeOut()
        {
            print(NetworkManager.ClientManager.Connection.Objects.Count + "daddyadaddy");
        }

        private void ObjectsOnOnPreDestroyClientObjects(NetworkConnection obj)
        {
            print("Pre Destroy Objects");
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
