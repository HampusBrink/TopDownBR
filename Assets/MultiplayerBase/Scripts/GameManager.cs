using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using Source.Scripts.NetworkRelated;
using Source.Scripts.Player;
using Source.Scripts.UI;
using TMPro;
using UnityEngine;
using static System.String;

namespace MultiplayerBase.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private UIScript _UI;
        [SerializeField] private TMP_Text _victoryRoyaleUI;
        public PowerupPopup powerupPopup;

        [SerializeField] private int _countDownTime = 20;

        public static GameManager Instance { get; private set; }
        
        [HideInInspector] public bool isDead;
        [HideInInspector] public List<PlayerStatus> alivePlayers;

        public event Action<List<PlayerStatus>> OnAlivePlayersChanged; 

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

        public override void OnStopServer()
        {
            base.OnStopServer();
            
            print("Stoping server");
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
            SRPC_StartGame();
            _UI.startGameObject.SetActive(false);
        }

        public void CheckForWinner()
        {
            if (alivePlayers.Count == 1)
            {
                
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SRPC_PlayerDied(PlayerStatus deadPlayer)
        {
            alivePlayers.Remove(deadPlayer);
            ORPC_UpdateAlivePlayers(alivePlayers);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SRPC_PlayerJoined(PlayerStatus player)
        {
            alivePlayers.Add(player);
            ORPC_UpdateAlivePlayers(alivePlayers);
        }

        [ObserversRpc]
        private void ORPC_UpdateAlivePlayers(List<PlayerStatus> players)
        {
            OnAlivePlayersChanged?.Invoke(players);
        }
        
        [ServerRpc]
        private void SRPC_DisplayVictor(string victorName)
        {
            ORPC_DisplayVictor(victorName);
        }

        // [ServerRpc]
        // void RPC_PlayerJoined()
        // {
        //     NetworkObject player = SpawnPointHandler.PlayerSpawn();
        //     _spawnPositions.RemoveAt(0);
        // }
        

        [ServerRpc(RequireOwnership = false)]
        private void SRPC_StartGame()
        {
            ORPC_StartGame();
        }

        [ObserversRpc]
        private void ORPC_DisplayVictor(string victorName)
        {
            if (victorName == Empty)
            {
                victorName = "Unknown Player";
            }

            _victoryRoyaleUI.text = victorName + " Wins!";
            _victoryRoyaleUI.gameObject.SetActive(true);
        }

        [ObserversRpc]
        private void ORPC_StartGame()
        {
            _timerStarted = true;
            _UI.timerDisplay.gameObject.SetActive(true);
        }
    }
}