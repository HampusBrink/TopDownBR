using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : MonoBehaviour
{
    public List<ItemParameter<object>> parameters;
    public SpriteRenderer spriteRenderer;

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
        player.itemManager.TakeItem(this);
        PickupEffectAndDestroy();
    }

    private void PickupEffectAndDestroy()
    {
        //todo: cool effect here
        Destroy(gameObject);
    }
}
