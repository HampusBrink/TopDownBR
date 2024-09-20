using Photon.Pun;
using TMPro;
using UnityEngine;
public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayText;

    private PhotonView _playerPv;

    private void Awake()
    {
        _playerPv = GetComponent<PhotonView>();
        _displayText.text = _playerPv.Controller.NickName;
    }
}