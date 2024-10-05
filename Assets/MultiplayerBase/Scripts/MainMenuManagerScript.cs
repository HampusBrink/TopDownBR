using System;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using Source.Scripts.NetworkRelated.Steam;
using UnityEngine;

namespace MultiplayerBase.Scripts
{
    public class MainMenuManagerScript : NetworkBehaviour
    {
        public static MainMenuManagerScript Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void OnPlayClicked()
        {
            SteamManager.JoinRandomOrCreateLobby();
        }
        public void OnQuitClicked()
        {
            Application.Quit();
        }

        public static void LoadScene(string sceneName)
        {
            var sld = new SceneLoadData(sceneName) { ReplaceScenes = ReplaceOption.All };
            Instance.SceneManager.LoadGlobalScenes(sld);
        }
    }
}