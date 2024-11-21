using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Item : MonoBehaviour
{
    public List<ItemParameter<object>> parameters;
    
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public Sprite GetSprite()
    {
        return _spriteRenderer.sprite;
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
