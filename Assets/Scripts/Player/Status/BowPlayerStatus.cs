using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class BowPlayerStatus : PlayerStatus
{
    public BowSpecificUpgrades bowSpecificUpgrades;
    
    [System.Serializable]
    public class BowSpecificUpgrades
    {
        public int bonusArrows = 0;
    }
}
