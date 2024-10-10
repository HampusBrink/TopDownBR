using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    private void Awake()
    {
        SpawnPointHandler.FetchSpawnPoints(_spawnPoints);
    }
}
