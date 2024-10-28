using UnityEngine;

[CreateAssetMenu(fileName = "LevelUpSO", menuName = "Scriptable Objects/LevelUpSO")]
public class LevelUpSO : ScriptableObject
{
    [Header("Sword")] 
    public int spinningBlades = 0;
    
    [Header("Bow")] 
    public int bonusArrows = 0;

}
