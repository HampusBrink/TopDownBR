using System;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using HeathenEngineering.SteamworksIntegration;
using MultiplayerBase.Scripts;
using Steamworks;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

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
        protected Callback<LobbyMatchList_t> Lobbies;
        
        public ulong CurrentLobbyID { get; private set; }


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
            Lobbies = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Could not create lobby!");
            }

            CurrentLobbyID = callback.m_ulSteamIDLobby;
            SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress", SteamUser.GetSteamID().ToString());
            _fishySteamworks.SetClientAddress(SteamUser.GetSteamID().ToString());
            _fishySteamworks.StartConnection(true);
        }

        private void OnJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            CurrentLobbyID = callback.m_ulSteamIDLobby;
            
            _fishySteamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress"));
            _fishySteamworks.StartConnection(false);
            MainMenuManagerScript.LoadScene("GameScene");
        }
        
        private void OnLobbyMatchList(LobbyMatchList_t callback)
        {
            if (callback.m_nLobbiesMatching < 1)
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 20);
                return;
            }

            for (int i = 0; i < callback.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "name");
                int playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
                int maxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);

                SteamMatchmaking.JoinLobby(lobbyID);
                Debug.Log($"Lobby {i}: Name: {lobbyName}, Players: {playerCount}/{maxPlayers}");
            }
        }

        public static void JoinRandomOrCreateLobby()
        {
            SteamMatchmaking.RequestLobbyList();
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}