using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class GiftSpawning : MonoBehaviour
{
    [SerializeField] private Transform[] allSpawnPositions;
    [SerializeField] private GameObject giftPrefab;
    [SerializeField] private float spawnInterval = 1f;

    private List<Transform> availableSpawnPositions;
    
    void Start()
    {
        if(!PhotonNetwork.IsMasterClient) return;

        availableSpawnPositions = new List<Transform>(allSpawnPositions);
        StartCoroutine(SpawnGiftRoutine());
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
        if (availableSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpawnPositions.Count);
            Transform spawnPosition = availableSpawnPositions[randomIndex];
            
            GameObject gift = PhotonNetwork.Instantiate(giftPrefab.name, spawnPosition.position, Quaternion.identity);
            gift.GetComponent<Gift>().SetSpawnPosition(spawnPosition);
            
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