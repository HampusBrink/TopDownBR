using FishNet.Object;
using FishNet.Transporting;
using LiteNetLib;

namespace NetworkRelated
{
    public class ServerManager : NetworkBehaviour
    {
        EventBasedNetListener eventListener = new ();

        private void Start()
        {
            
            eventListener.PeerDisconnectedEvent += EventListenerOnPeerDisconnectedEvent;
            NetworkManager.TransportManager.Transport.OnClientConnectionState += Transport_OnClientConnectionState;
            ServerManager.OnServerConnectionState += ServerManagerOnOnServerConnectionState;
            
        }

        private void EventListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            print(peer);
            print(disconnectinfo);
        }

        private void ServerManagerOnOnServerConnectionState(ServerConnectionStateArgs obj)
        {
            print("BALLOCKS");
        }

        private void Transport_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            
        }
    }
}
