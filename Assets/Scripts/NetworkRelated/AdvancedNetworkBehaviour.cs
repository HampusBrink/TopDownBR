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

        // public override void OnStopClient()
        // {
        //     base.OnStopClient();
        //     
        //     print("Stop Client");
        //     NetworkManager.ClientManager.OnAuthenticated -= OnClientAuth;
        // }

        private void OnClientAuth()
        {
            print("Client Auth");
            RespawnObject();
        }

        /// <summary>
        /// Respawns this object when a new connection has been established.
        /// </summary>
        private void RespawnObject()
        {
            if(!ClientInitialized) return;
            
            print("Respawn Object");
            SRPC_SpawnObject();
            gameObject.SetActive(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SRPC_SpawnObject()
        {
            print("Spawn Object");
            NetworkManagerRef.ServerManager.Spawn(NetworkObject,LocalConnection);
        }
    }
}
