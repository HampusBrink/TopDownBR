using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItem : MonoBehaviour
{
    public int amount;
    [System.NonSerialized] public Image image;

    public void SetImage(Sprite sprite)
    {
        if (image == null)
            image = GetComponent<Image>();
        image.sprite = sprite;
    }
}
