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
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            NetworkManager.ClientManager.OnAuthenticated += OnClientAuth;
            if (IsOwner) ClientInitialized = true;
        }
        
        private void OnClientAuth()
        {
            if(!ClientInitialized) return;
            
            ServerManager.Spawn(NetworkObject,LocalConnection);
            gameObject.SetActive(true);
        }
    }
}
