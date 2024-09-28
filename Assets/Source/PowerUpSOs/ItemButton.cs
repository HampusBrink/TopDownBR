using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public PowerupSO powerUp;
    private Image _display;
    private void Start()
    {
        _display = GetComponent<Image>();
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
        playerStatus.bonusDamagePercent = powerUp.bonusDamagePercent;
    }
}
