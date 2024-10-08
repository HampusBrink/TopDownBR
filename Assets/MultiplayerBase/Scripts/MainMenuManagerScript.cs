using System;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using Source.Scripts.NetworkRelated.Steam;
using UnityEngine;

namespace MultiplayerBase.Scripts
{
    public class MainMenuManagerScript : MonoBehaviour
    {
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
            RogueRoyaleNetworkManager.Instance.NetworkManager.SceneManager.LoadGlobalScenes(sld);
        }
    }
}
