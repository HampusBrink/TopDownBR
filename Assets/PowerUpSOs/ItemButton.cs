using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public PowerupSO powerUp;
    [SerializeField] private Image _display;
    [SerializeField] private TMP_Text _displayText;
    private void Start()
    {
        _display.sprite = powerUp.display;
        _displayText.text = powerUp.description;
    }

    public void UpdateItem()
    {
        _display.sprite = powerUp.display;
        _displayText.text = powerUp.description;
    }

    public void OnItemPicked()
    {
        var playerStatus = GameManager.Instance._localPlayer.GetComponent<PlayerStatus>();
        if(!playerStatus) return;
        AddNewBuffs(playerStatus,powerUp);
    }

    void AddNewBuffs(PlayerStatus playerStatus, PowerupSO powerUp)
    {
        playerStatus.maxHealth += powerUp.bonusMaxHealthFlat;
        playerStatus.attackDamageMultiplier += powerUp.bonusDamagePercent;
        playerStatus.movementSpeedMultiplier += powerUp.bonusAttackSpeedPercent;
        playerStatus.attackSpeedMultiplier += powerUp.bonusAttackSpeedPercent;
        playerStatus.weaponLengthMultiplier += powerUp.bonusWeaponLengthPercent;
        if (powerUp.instantHealth > 0)
        {
            playerStatus.pv.RPC(nameof(playerStatus.Heal),RpcTarget.All,powerUp.instantHealth);
        }
    }
}
