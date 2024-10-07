using Source.Scripts.Player;
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
        // var playerStatus = GameManager.Instance._localPlayer.GetComponent<PlayerStatus>();
        // if(!playerStatus) return;
        // AddNewBuffs(playerStatus,powerUp);
    }

    void AddNewBuffs(PlayerStatus playerStatus, PowerupSO powerUpSO)
    {
        playerStatus.maxHealth += powerUpSO.bonusMaxHealthFlat;
        playerStatus.attackDamageMultiplier += powerUpSO.bonusDamagePercent;
        playerStatus.movementSpeedMultiplier += powerUpSO.bonusAttackSpeedPercent;
        playerStatus.attackSpeedMultiplier += powerUpSO.bonusAttackSpeedPercent;
        playerStatus.weaponLengthMultiplier += powerUpSO.bonusWeaponLengthPercent;
        if (powerUpSO.instantHealth > 0)
        {
            playerStatus.Heal(powerUpSO.instantHealth);
        }
    }
}
