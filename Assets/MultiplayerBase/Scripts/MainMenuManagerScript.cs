using System;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using Source.Scripts.NetworkRelated;
using Source.Scripts.NetworkRelated.Steam;
using UnityEngine;

namespace MultiplayerBase.Scripts
{
    public class MainMenuManagerScript : MonoBehaviour
    {
        private ClientConnectionStateArgs _clientConnectionStateArgs;
        private void OnEnable()
        {
            RogueRoyaleNetworkManager.Instance.NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        private void OnDisable()
        {
            RogueRoyaleNetworkManager.Instance.NetworkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
        }

        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            _clientConnectionStateArgs = obj;
            if(obj.ConnectionState != LocalConnectionState.Started) return;
            
            LoadScene("GameScene");
        }

        public void OnPlayClicked()
        {
            if (RogueRoyaleNetworkManager.Instance.GetComponent<FishySteamworks.FishySteamworks>())
            {
                SteamManager.JoinRandomOrCreateLobby();
            }
            else
            {
                
                if (!RogueRoyaleNetworkManager.Instance.NetworkManager.IsServerStarted)
                {
                    RogueRoyaleNetworkManager.Instance.NetworkManager.ServerManager.StartConnection();
                }

                RogueRoyaleNetworkManager.Instance.NetworkManager.ClientManager.StartConnection();
            }
        }

        public void OnQuitClicked()
        {
            Application.Quit();
        }

        public static void LoadScene(string sceneName)
        {
            var sld = new SceneLoadData(sceneName) { ReplaceScenes = ReplaceOption.All };
            RogueRoyaleNetworkManager.Instance.NetworkManager.SceneManager.LoadGlobalScenes(sld);
        }
    }
}