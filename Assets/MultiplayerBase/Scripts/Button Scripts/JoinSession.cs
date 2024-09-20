using Photon.Pun;
using TMPro;
using UnityEngine;
public class JoinSession : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _displayText;
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_displayText.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}