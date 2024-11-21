using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : MonoBehaviour
{
    public List<ItemParameter<object>> parameters;
    
    public SpriteRenderer spriteRenderer;

    

    public void TakeItem(PlayerStatus player)
    {
        player.itemManager.TakeItem(this);
        PickupEffectAndDestroy();
    }

    private void PickupEffectAndDestroy()
    {
        //todo: cool effect here
        Destroy(gameObject);
    }
}
