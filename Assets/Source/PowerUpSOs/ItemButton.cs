using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public PowerupSO powerUp;
    [SerializeField] private Image _display;
    private void Start()
    {
        _display.sprite = powerUp.display;
    }

    public void UpdateItem()
    {
        _display.sprite = powerUp.display;
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
    }
}
