using FishNet.Component.Spawning;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace NetworkRelated
{
    public class RogueRoyaleNetworkManager : MonoBehaviour
    {
        private PlayerSpawner _spawner;
        private NetworkObject _player;

        private NetworkManager _networkManager;

        public NetworkManager NetworkManager => _networkManager;
    
        public static RogueRoyaleNetworkManager Instance;
        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
            Instance = this;
            _spawner = GetComponent<PlayerSpawner>();
        }

        private void OnEnable()
        {
            SpawnPointHandler.OnFetchSpawnPoints += OnFetchSpawnPoints;
            SpawnPointHandler.OnPlayerSpawned += OnPlayerSpawned;
            _spawner.OnSpawned += FishySpawn;
        }

        private void OnDisable()
        {
            SpawnPointHandler.OnFetchSpawnPoints -= OnFetchSpawnPoints;
            SpawnPointHandler.OnPlayerSpawned -= OnPlayerSpawned;
            _spawner.OnSpawned -= FishySpawn;
        }

        private void OnFetchSpawnPoints(Transform[] spawnPoints)
        {
            _spawner.Spawns = spawnPoints;
        }
    
        private void FishySpawn(NetworkObject obj)
        {
            _player = obj;
        }

        private NetworkObject OnPlayerSpawned() => _player;
    }
}
