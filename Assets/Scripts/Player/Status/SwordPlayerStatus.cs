using Player;
using UnityEngine;

public class SwordPlayerStatus : PlayerStatus
{
    public SwordSpecificUpgrades swordSpecificUpgrades;
    
    [System.Serializable]
    public class SwordSpecificUpgrades
    {
        public int spinningBlades = 0;
    }
}
