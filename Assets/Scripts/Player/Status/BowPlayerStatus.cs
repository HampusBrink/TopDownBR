using Player;
using UnityEngine;

public class BowPlayerStatus : PlayerStatus
{
    public BowSpecificUpgrades bowSpecificUpgrades;
    
    [System.Serializable]
    public class BowSpecificUpgrades
    {
        public int bonusArrows = 0;
    }
}
