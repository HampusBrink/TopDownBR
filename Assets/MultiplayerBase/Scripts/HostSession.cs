using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
public class HostSession : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int _maxPlayers = 7;
    public void CreateRoom()
    {
        var options = new RoomOptions
        {
            MaxPlayers = _maxPlayers
        };
        PhotonNetwork.CreateRoom(_inputField.text, options);
    }
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}