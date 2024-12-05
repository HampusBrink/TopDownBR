using FishNet.Managing;
using FishNet.Object;

namespace NetworkRelated
{
    /// <summary>
    /// A normal NetworkBehaviour with additional features like Host Migration.
    /// </summary>
    public class AdvancedNetworkBehaviour : NetworkBehaviour
    {
        /// <summary>
        /// True for client that initialized this object.
        /// </summary>
        protected bool ClientInitialized;

        protected NetworkManager NetworkManagerRef;
        public override void OnStartClient()
        {
            base.OnStartClient();

            NetworkManagerRef = NetworkManager;
            NetworkManager.ClientManager.OnAuthenticated += OnClientAuth;
            if (IsOwner) ClientInitialized = true;
            print("Start Client");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            print(IsServerInitialized + gameObject.name);
            print("SERVER IS STARTED");
        }

        // public override void OnStopClient()
        // {
        //     base.OnStopClient();
        //     
        //     print("Stop Client");
        //     NetworkManager.ClientManager.OnAuthenticated -= OnClientAuth;
        // }

        private void OnClientAuth()
        {
           // if(!IsOwner || !IsServerInitialized) return;
            print("Client Auth");
            RespawnObject();
        }

        /// <summary>
        /// Respawns this object when a new connection has been established.
        /// </summary>
        private void RespawnObject()
        {
            if(!ClientInitialized) return;
            
            SRPC_SpawnObject();
            gameObject.SetActive(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SRPC_SpawnObject()
        {
            NetworkManagerRef.ServerManager.Spawn(NetworkObject,LocalConnection);
        }
    }
}
