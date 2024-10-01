using Photon.Pun;
using TMPro;
using UnityEngine;

public class JoinSession : MonoBehaviourPunCallbacks
{
    public const string QUEUE_PROP_KEY = "Queue";
    
    [SerializeField] private TMP_Text _displayText;
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_displayText.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Room is full, add player to queue
        string playerId = PhotonNetwork.LocalPlayer.UserId;
        ExitGames.Client.Photon.Hashtable queueProperty = new ExitGames.Client.Photon.Hashtable();
        queueProperty[QUEUE_PROP_KEY] = playerId;
        PhotonNetwork.CurrentRoom.SetCustomProperties(queueProperty);

    }
}