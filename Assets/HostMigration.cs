using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

public class HostMigration : MonoBehaviour
{
    private void Awake()
    {
        ServerManager sm = GetComponent<ServerManager>();
        sm.Objects.OnPreDestroyClientObjects += Objects_OnPreDestroyClientObjects;
    }

    protected virtual void Objects_OnPreDestroyClientObjects(NetworkConnection conn)
    {
        foreach (NetworkObject networkObject in conn.Objects)
            networkObject.RemoveOwnership();
    }
}
