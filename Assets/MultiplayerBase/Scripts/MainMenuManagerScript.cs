using Photon.Pun;
using Photon.Realtime;
using Source.Scripts.NetworkRelated.Steam;
using UnityEngine;

namespace MultiplayerBase.Scripts
{
    public class MainMenuManagerScript : MonoBehaviourPunCallbacks
    {
        public const string QUEUE_PROP_KEY = "Queue";
        
        [SerializeField] private int _maxPlayers = 21;
        public void OnPlayClicked()
        {
            SteamManager.JoinRandomOrCreateLobby();
        }
        public void OnQuitClicked()
        {
            Application.Quit();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            // Room is full, add player to queue
            string playerId = PhotonNetwork.LocalPlayer.UserId;
            ExitGames.Client.Photon.Hashtable queueProperty = new ExitGames.Client.Photon.Hashtable();
            queueProperty[QUEUE_PROP_KEY] = playerId;
            PhotonNetwork.CurrentRoom.SetCustomProperties(queueProperty);
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}