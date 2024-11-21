using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItem : MonoBehaviour
{
    public int amount;
    public TextMeshProUGUI amountText;
    [System.NonSerialized] public Image image;

    public void SetImage(Sprite sprite)
    {
        if (image == null)
            image = GetComponent<Image>();
        image.sprite = sprite;
    }

    public void UpdateAmount()
    {
        amount++;
        amountText.text = amount.ToString();
    }
}
