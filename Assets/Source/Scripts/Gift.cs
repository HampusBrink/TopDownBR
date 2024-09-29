using System;
using Photon.Pun;
using UnityEngine;

public class Gift : MonoBehaviour
{
    private Transform spawnPosition;
    private GiftSpawning spawner;

    void Start()
    {
        spawner = FindObjectOfType<GiftSpawning>();
    }

    public void SetSpawnPosition(Transform position)
    {
        spawnPosition = position;
    }

    public void Collect()
    {
        spawner.ReturnSpawnPosition(spawnPosition);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out PhotonView photonView))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Collect();
            }
            if(!photonView.IsMine) return;

            photonView.GetComponent<PlayerMovement>().disableMovement = true;
            GameManager.Instance.PowerupPopup.gameObject.SetActive(true);
        }
    }
}