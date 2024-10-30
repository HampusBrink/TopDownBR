using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class SwordPlayerStatus : PlayerStatus
{
    public SwordSpecificUpgrades swordSpecificUpgrades;
    
    [System.Serializable]
    public class SwordSpecificUpgrades
    {
        public int spinningBlades = 0;
    }
}
