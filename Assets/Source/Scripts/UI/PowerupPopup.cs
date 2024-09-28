using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPopup : MonoBehaviour
{
    private bool _closeMenu;
    public float popupSpeed = 0.1f;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        _closeMenu = false;
    }

    public void OnButtonPressed()
    {
        _closeMenu = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _closeMenu ? Vector3.zero : Vector3.one, popupSpeed);
        if(transform.localScale.magnitude < new Vector3(0.1f,0.1f,0.1f).magnitude) gameObject.SetActive(false);
    }
}
