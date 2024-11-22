using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

public class HostMigration : MonoBehaviour
{
    public static HostMigration instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void InitializeOnServer(NetworkObject obj)
    {
        GetComponent<ServerManager>().Spawn(obj, obj.LocalConnection);
    }
}
