using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using LiteNetLib;
using UnityEngine;

namespace NetworkRelated
{
    public class ServerManager : NetworkBehaviour
    {
        EventBasedNetListener eventListener = new ();

        private void Start()
        {
            ServerManager.SetRemoteClientTimeout(RemoteTimeoutType.Disabled,10);
            eventListener.PeerDisconnectedEvent += EventListenerOnPeerDisconnectedEvent;
            
            
            NetworkManager.TransportManager.Transport.OnClientConnectionState += Transport_OnClientConnectionState;
            ServerManager.OnServerConnectionState += ServerManagerOnOnServerConnectionState;
        }

        private void ClientManagerOnOnClientConnectionState(ClientConnectionStateArgs obj)
        {
            
        }

        private void EventListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            Debug.Log("hejsan");
        }

        private void ServerManagerOnOnServerConnectionState(ServerConnectionStateArgs obj)
        {
            
        }

        private void Transport_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            
        }

        public void LeaveGame()
        {
            
            ServerManager.StopConnection(true);
        }
    }
}
