using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItem : MonoBehaviour
{
    public int amount;
    public TextMeshProUGUI amountText;
    [System.NonSerialized] public Image image;
    public Image borderImage;

    public void SetImage(Sprite sprite)
    {
        if (image == null)
            image = GetComponent<Image>();
        image.sprite = sprite;
    }
    
    public void SetBorder(Sprite sprite, Color color)
    {
        if (borderImage != null)
            borderImage.sprite = sprite;
        borderImage.color = color;
    }

    public void UpdateAmount()
    {
        amount++;
        amountText.text = amount.ToString();
    }
}
