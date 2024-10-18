using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public static class SpawnPointHandler
{
    public static event Action<Transform[]> OnFetchSpawnPoints;
    public static event Func<int> OnFetchClientID;
    public static event Func<NetworkObject> OnPlayerSpawned; 
    public static void FetchSpawnPoints(Transform[] spawnPoints) => OnFetchSpawnPoints?.Invoke(spawnPoints);

    public static int FetchClientID() => OnFetchClientID?.Invoke() ?? 0;
    public static NetworkObject PlayerSpawn() => OnPlayerSpawned?.Invoke();
}
