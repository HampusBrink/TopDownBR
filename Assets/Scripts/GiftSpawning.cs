using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using MultiplayerBase.Scripts;

public class GiftSpawning : NetworkBehaviour
{
    [SerializeField] private Transform[] allSpawnPositions;
    [SerializeField] private Item[] spawnableItems;
    [SerializeField] private float spawnInterval = 1f;

    private List<Transform> availableSpawnPositions;


    public override void OnStartServer()
    {
        base.OnStartServer();
        
        availableSpawnPositions = new List<Transform>(allSpawnPositions);
        StartCoroutine(SpawnGiftRoutine());
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

    private IEnumerator SpawnGiftRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnGift();
        }
    }

    private void SpawnGift()
    {
        if (!GameManager.Instance.GameStarted) return;
        if (availableSpawnPositions.Count > 0)
        {
            print("Rafa har små bollar");
            int randomIndex = Random.Range(0, availableSpawnPositions.Count);
            Transform spawnPosition = availableSpawnPositions[randomIndex];

            var spawnableItem = GetRandomItem(spawnableItems);
            
            GameObject gift = Instantiate(spawnableItem.gameObject, spawnPosition.position, Quaternion.identity);
            NetworkManager.ServerManager.Spawn(gift);
            
            availableSpawnPositions.RemoveAt(randomIndex);
        }
    }

    public void ReturnSpawnPosition(Transform spawnPosition)
    {
        if (!availableSpawnPositions.Contains(spawnPosition))
        {
            availableSpawnPositions.Add(spawnPosition);
        }
    }
}