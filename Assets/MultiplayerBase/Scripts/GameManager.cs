using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Source.Scripts.UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static System.String;

namespace MultiplayerBase.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private List<Transform> _spawnPositions;
        [SerializeField] private UIScript _UI;
        [SerializeField] private TMP_Text _victoryRoyaleUI;
        public PowerupPopup powerupPopup;

        [SerializeField] private int _countDownTime = 20;

        public static GameManager Instance { get; private set; }
        
        [HideInInspector] public bool isDead;
        [HideInInspector] public List<PlayerMovement> alivePlayers;

        public bool GameStarted { get; private set; }

        //remove later
        public bool isTestScene;

        private bool _timerStarted;
        private float _timer;

        public float Timer => Mathf.Abs(_timer - _countDownTime);
        
        void Awake()
        {
            if (isTestScene) GameStarted = true;
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            RogueRoyaleNetworkManager.Instance.NetworkManager.TimeManager.OnTick += ServerTick;
        }

        private void ServerTick()
        {
            if (_timerStarted)
            {
                _timer += (float)TimeManager.TickDelta;

                if (_timer <= _countDownTime) return;
                _timer = 0;
                _timerStarted = false;
                GameStarted = true;

                _UI.timerDisplay.gameObject.SetActive(false);
            }
        }

        public void OnStartGameClicked()
        {
            RPC_StartGame();
            _UI.startGameObject.SetActive(false);
        }

        public void CheckForWinner()
        {
            if (alivePlayers.Count == 1)
            {
                
            }
        }

        public void PlayerDied()
        {
            alivePlayers = FindObjectsOfType<PlayerMovement>().ToList();
        }
        
        [ServerRpc]
        void RPC_DisplayVictor(string victorName)
        {
            DisplayVictor(victorName);
        }

        // [ServerRpc]
        // void RPC_PlayerJoined()
        // {
        //     NetworkObject player = SpawnPointHandler.PlayerSpawn();
        //     _spawnPositions.RemoveAt(0);
        // }
        

        [ServerRpc(RequireOwnership = false)]
        public void RPC_StartGame()
        {
            StartGame();
        }

        [ObserversRpc]
        void DisplayVictor(string victorName)
        {
            if (victorName == Empty)
            {
                victorName = "Unknown Player";
            }

            _victoryRoyaleUI.text = victorName + " Wins!";
            _victoryRoyaleUI.gameObject.SetActive(true);
        }

        [ObserversRpc]
        public void StartGame()
        {
            _timerStarted = true;
            _UI.timerDisplay.gameObject.SetActive(true);
        }
    }
}