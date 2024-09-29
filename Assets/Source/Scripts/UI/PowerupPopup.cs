using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerupPopup : MonoBehaviour
{
    private bool _closeMenu;
    public float popupSpeed = 0.1f;

    public PowerupSO[] powerUps;
    
    private void OnEnable()
    {
        print("dad");
        _closeMenu = false;

        var items = GetComponentsInChildren<ItemButton>();
        foreach (var item in items)
        {
            var randomPowerUp = Random.Range(0, powerUps.Length);
            item.powerUp = powerUps[randomPowerUp];
            item.UpdateItem();
        }
    }

    public void OnButtonPressed()
    {
        _closeMenu = true;
    }
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _closeMenu ? Vector3.zero : Vector3.one, popupSpeed);
        if(transform.localScale.magnitude < new Vector3(0.1f,0.1f,0.1f).magnitude) gameObject.SetActive(false);
    }
}
