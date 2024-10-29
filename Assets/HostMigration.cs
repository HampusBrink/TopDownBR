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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        print(GetComponent<NetworkManager>());
        print(GetComponent<ServerManager>());
    }

    public void InitializeOnServer(NetworkObject obj)
    {
        GetComponent<ServerManager>().Spawn(obj, obj.LocalConnection);
    }
}
