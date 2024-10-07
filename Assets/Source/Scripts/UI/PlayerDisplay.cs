using System;
using FishNet.Object;
using TMPro;
using UnityEngine;
public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _displayText;

    private NetworkObject _playerNo;

    private void Awake()
    {
        _playerNo = GetComponentInParent<NetworkObject>();
        if(_playerNo.ClientManager == null) return;
        var displayName = _playerNo.ClientManager.name;
        
        if (displayName == string.Empty) return;
        _displayText.text = displayName;
    }
}