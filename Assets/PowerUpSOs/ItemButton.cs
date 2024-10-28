using MultiplayerBase.Scripts;
using Player;
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
        var playerStatus = GameManager.Instance.localPlayer;
        if(!playerStatus) return;
        AddNewBuffs(playerStatus,powerUp);
    }

    void AddNewBuffs(PlayerStatus playerStatus, PowerupSO powerUpSO)
    {
        playerStatus.playerStatMultipliers.maxHealth += powerUp.bonusMaxHealthFlat;
        playerStatus.weaponStatMultipliers.attackDamageMultiplier += powerUp.bonusDamagePercent;
        playerStatus.movementStatMultipliers.movementSpeedMultiplier += powerUp.bonusMovementSpeedPercent;
        playerStatus.weaponStatMultipliers.attackSpeedMultiplier += powerUp.bonusAttackSpeedPercent;
        playerStatus.weaponStatMultipliers.attackRangeMultiplier += powerUp.bonusWeaponLengthPercent; // change name of this later
        if (powerUp.instantHealth > 0)
        {
            playerStatus.Heal(powerUpSO.instantHealth);
        }
    }

    void AddSpecificBuffs(PlayerStatus playerStatus, LevelUpSO levelUpSO)
    {
        switch (playerStatus)
        {
            case SwordPlayerStatus swordPlayerStatus:
                swordPlayerStatus.swordSpecificUpgrades.spinningBlades = levelUpSO.spinningBlades;
                break;

            case BowPlayerStatus bowPlayerStatus:
                bowPlayerStatus.bowSpecificUpgrades.bonusArrows = levelUpSO.bonusArrows;
                break;

            default:
                Debug.LogWarning("Unknown player status type.");
                break;
        }
    }
}
