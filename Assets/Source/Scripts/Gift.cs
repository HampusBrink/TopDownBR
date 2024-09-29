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
        Destroy(gameObject);
    }
}