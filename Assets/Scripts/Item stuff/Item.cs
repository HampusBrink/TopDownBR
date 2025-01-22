using System;
using System.Collections.Generic;
using FishNet.Object;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : NetworkBehaviour
{
    public List<ItemParameter<object>> parameters;
    public SpriteRenderer spriteRenderer;

    private bool isPendingKill;

    public int weight;

    public Sprite GetBorderSprite()
    {
        return GetComponent<ItemVisuals>().borderSprite;
    }

    public Vector4 GetBorderColor()
    {
        return GetComponent<ItemVisuals>().defaultColor;
    }

    public void TakeItem(PlayerStatus player)
    {
        if(isPendingKill) return;
        player.itemManager.TakeItem(this);
        isPendingKill = true;
        PickupEffectAndDestroy();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PickupEffectAndDestroy()
    {
        //todo: cool effect here
        NetworkManager.ServerManager.Despawn(gameObject);
    }
}
