using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerItem : MonoBehaviour
{
    public Item item;
    public TextMeshProUGUI nameText;
    public Action<Item> OnPick = delegate { };
    private Image _uiImage;
    

    public void Init(Item i)
    {
        item = i;
        _uiImage = GetComponent<Image>();
        SetUIImage();
        nameText.text = item.name;
    }

    private void SetUIImage()
    {
        _uiImage.sprite = item.spriteRenderer.sprite;
    }

    public void PickItem()
    {
        Item i = Instantiate(item);
        i.name = item.name;
        OnPick.Invoke(i);
        Destroy(i.gameObject);
    }
}
