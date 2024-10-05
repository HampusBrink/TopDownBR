using System;
using FishNet.Managing;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Scripts.NetworkRelated.Steam
{
    public class SteamManager : SteamworksBehaviour
    {
        public static SteamManager Instance { get; private set; }
        
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private FishySteamworks.FishySteamworks _fishySteamworks;

        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> JoinRequested;
        protected Callback<LobbyEnter_t> LobbyEnter;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            JoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
            LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        }
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            Debug.Log("Lobby Creation Result Code : " + callback.m_eResult);
        }

        private void OnJoinRequested(GameLobbyJoinRequested_t callback)
        {
        }

        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            
        }

        public static void JoinRandomOrCreateLobby()
        {
            SteamMatchmaking.RequestLobbyList();
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
        }
        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
