using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSpecificUpgradeSO", menuName = "Scriptable Objects/WeaponSpecificUpgradeSO")]
public class WeaponSpecificUpgradeSO : ScriptableObject
{
    [Header("Sword")] 
    public int spinningBlades = 0;
    
    [Header("Bow")] 
    public int bonusArrows = 0;

}
