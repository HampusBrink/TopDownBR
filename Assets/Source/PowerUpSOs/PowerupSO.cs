using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ItemSOs/Item")]
public class PowerupSO : ScriptableObject
{
    public Sprite display;
    public float bonusDamagePercent;
    public float bonusMaxHealthFlat;
}
