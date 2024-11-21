using MultiplayerBase.Scripts;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public GenericUpgradeSO powerUp;
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

    void AddNewBuffs(PlayerStatus playerStatus, GenericUpgradeSO genericUpgrade)
    {
        playerStatus.vitalUpgrades.maxHealth += powerUp.bonusMaxHealthFlat;
        playerStatus.combatUpgrades.attackDamageMultiplier += powerUp.bonusDamagePercent;
        //playerStatus.movementUpgrades.movementSpeedMultiplier += powerUp.bonusMovementSpeedPercent;
        playerStatus.combatUpgrades.attackSpeedMultiplier += powerUp.bonusAttackSpeedPercent;
        playerStatus.combatUpgrades.attackRangeMultiplier += powerUp.bonusWeaponLengthPercent; // change name of this later
        if (powerUp.instantHealth > 0)
        {
            playerStatus.Heal(genericUpgrade.instantHealth);
        }
    }

    void AddSpecificBuffs(PlayerStatus playerStatus, WeaponSpecificUpgradeSO weaponSpecificUpgrade)
    {
        switch (playerStatus)
        {
            case SwordPlayerStatus swordPlayerStatus:
                swordPlayerStatus.swordSpecificUpgrades.spinningBlades = weaponSpecificUpgrade.spinningBlades;
                break;

            case BowPlayerStatus bowPlayerStatus:
                bowPlayerStatus.bowSpecificUpgrades.bonusArrows = weaponSpecificUpgrade.bonusArrows;
                break;

            default:
                Debug.LogWarning("Unknown player status type.");
                break;
        }
    }
}
