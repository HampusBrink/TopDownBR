using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MultiplayerBase.Scripts
{
    public class MainMenuManagerScript : MonoBehaviourPunCallbacks
    {
        public const string QUEUE_PROP_KEY = "Queue";
        
        [SerializeField] private int _maxPlayers = 21;
        public void OnPlayClicked()
        {
            if(PhotonNetwork.CountOfRooms<= 0)
            {
                var options = new RoomOptions
                {
                    MaxPlayers = _maxPlayers
                };
                PhotonNetwork.JoinRandomOrCreateRoom(roomOptions:options);
                return;
            }
            
            PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork.JoinRandomOrCreateRoom(roomOptions:options);
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