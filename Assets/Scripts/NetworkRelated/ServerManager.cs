using System.Linq;
using FishNet.Object;
using UnityEngine;

namespace NetworkRelated
{
    public class ServerManager : NetworkBehaviour
    {
        public override void OnStopServer()
        {
            base.OnStopServer();
            print("Client");

            if(!IsServerInitialized) return;

            InitializeHostMigration();
        }

        private void InitializeHostMigration()
        {
            ORPC_AssignNewHost();
            print("Server");
            
            throw new System.NotImplementedException();
        }

        [ObserversRpc]
        void ORPC_AssignNewHost()
        {
            print("RPC");
            NetworkManager.ServerManager.StartConnection();
            NetworkManager.ClientManager.StartConnection();
        }
    }
}
