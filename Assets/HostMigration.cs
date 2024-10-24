using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

public class HostMigration : MonoBehaviour
{
    private void Start()
    {
        ServerManager sm = GetComponent<ServerManager>();
        sm.Objects.OnPreDestroyClientObjects += Objects_OnPreDestroyClientObjects;
    }

    private void Objects_OnPreDestroyClientObjects(NetworkConnection conn)
    {
        print("YEEPERS");
        foreach (NetworkObject networkObject in conn.Objects)
            networkObject.RemoveOwnership();
    }
}
