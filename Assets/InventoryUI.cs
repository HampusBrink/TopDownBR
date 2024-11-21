using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public ItemManager itemManager;
    public RectTransform inventoryContent;
    public DisplayItem itemDisplayPrefab;

    private void OnEnable()
    {
        itemManager.OnItemAdded += AddItem;
    }


    private void AddItem(Item item)
    {
        foreach (Transform child in inventoryContent)
        {
            if (child.name.Equals(item.name))
            {
                child.GetComponent<DisplayItem>().UpdateAmount();
                return;
            }
        }
        DisplayItem displayItem = Instantiate(itemDisplayPrefab, inventoryContent);
        displayItem.SetImage(item.GetSprite());
        displayItem.gameObject.name = item.name;
    }
}
