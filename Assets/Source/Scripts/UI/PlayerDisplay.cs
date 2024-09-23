using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayText;

    private PhotonView _playerPv;

    private void Awake()
    {
        _playerPv = GetComponentInParent<PhotonView>();
        var displayName = _playerPv.Controller.NickName;
        
        if (displayName == string.Empty) return;
        _displayText.text = displayName;
    }
}