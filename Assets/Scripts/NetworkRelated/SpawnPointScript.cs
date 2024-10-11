using UnityEngine;

namespace NetworkRelated
{
    public class SpawnPointScript : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoints;
        private void Awake()
        {
            SpawnPointHandler.FetchSpawnPoints(_spawnPoints);
        }
    }
}
