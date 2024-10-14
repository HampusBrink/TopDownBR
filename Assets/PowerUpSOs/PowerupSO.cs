using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ItemSOs/Item")]
public class PowerupSO : ScriptableObject
{
    public Sprite display;
    public string description = "this is an item";
    
    public float bonusDamagePercent;
    public float bonusMaxHealthFlat;
    public float bonusAttackSpeedPercent;
    public float instantHealth;
    public float bonusWeaponLengthPercent;
    public float bonusMovementSpeedPercent;
}