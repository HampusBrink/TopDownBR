using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using MultiplayerBase.Scripts;
using UnityEngine.Serialization;

public class ChestSpawner : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private ItemContainer chestPrefab;
    [SerializeField] private float spawnInterval = 1f;

    private List<Transform> _availableSpawnPositions;
    private List<ItemContainer> _spawnedChests = new ();

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        _availableSpawnPositions = new List<Transform>(spawnPoints);
        SpawnChests(spawnPoints.Length / 2);
    }

    private void SpawnChests(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            if (_availableSpawnPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, _availableSpawnPositions.Count);
                Transform spawnPosition = _availableSpawnPositions[randomIndex];
            
                ItemContainer chest = Instantiate(chestPrefab, spawnPosition.position, Quaternion.identity);
                _spawnedChests.Add(chest);
                
                NetworkManager.ServerManager.Spawn(chest.gameObject);
            
                _availableSpawnPositions.RemoveAt(randomIndex);
            }
        }
    }

    private void RespawnChests()
    {
        for (var i = 0; i < _spawnedChests.Count; i++)
        {
            _spawnedChests[i].SRPC_SetChestOpened(false);
        }
    }

    public static Item GetRandomItem(Item[] items)
    {
        var totalWeight = 0;
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }

        var randomWeight = UnityEngine.Random.Range(0f, totalWeight);
        
        var cumulativeWeight = 0;
        foreach (var item in items)
        {
            cumulativeWeight += item.weight;
            if (randomWeight <= cumulativeWeight)
            {
                return item;
            }
        }

        return null;
    }

    private void SpawnGift()
    {
        
    }

    public void ReturnSpawnPosition(Transform spawnPosition)
    {
        if (!_availableSpawnPositions.Contains(spawnPosition))
        {
            _availableSpawnPositions.Add(spawnPosition);
        }
    }
}