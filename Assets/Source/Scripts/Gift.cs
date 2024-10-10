using System;
using FishNet.Object;
using MultiplayerBase.Scripts;
using Source.Scripts.NetworkRelated;
using UnityEngine;

public class Gift : NetworkBehaviour
{
    private Transform spawnPosition;
    private GiftSpawning spawner;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        spawner = FindObjectOfType<GiftSpawning>();
    }

    public void SetSpawnPosition(Transform position)
    {
        spawnPosition = position;
    }

    public void Collect()
    {
        spawner.ReturnSpawnPosition(spawnPosition);
        Despawn(gameObject, DespawnType.Pool);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out NetworkObject networkObject))
        {
            if (networkObject.IsServerInitialized)
            {
                Collect();
            }
            if(!networkObject.IsOwner) return;

            GameManager.Instance.powerupPopup.gameObject.SetActive(true);
        }
    }
}