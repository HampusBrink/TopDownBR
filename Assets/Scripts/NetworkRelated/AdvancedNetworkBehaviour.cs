using System;
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
        private void OnEnable()
        {
            NetworkRelated.ServerManager.Instance.ConnectedToServer += OnClientAuth;
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            NetworkRelated.ServerManager.Instance.ConnectedToServer += OnClientAuth;
            
            NetworkManager.ClientManager.OnAuthenticated += OnClientAuth;
            if (IsOwner) ClientInitialized = true;
        }
        
        private void OnClientAuth()
        {
            print("Client Auth");
            if(!ClientInitialized) return;
            
            print(ServerManager + "Server manager");
            
            HostMigration.instance.InitializeOnServer(NetworkObject);
            gameObject.SetActive(true);
        }
    }
}
