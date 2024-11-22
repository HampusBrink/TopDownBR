using System;
using MultiplayerBase.Scripts;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public InputActionReference showInventory;
    public ItemManager itemManager;
    public RectTransform inventoryContent;
    public RectTransform inventoryMenu, statsMenu;
    public DisplayItem itemDisplayPrefab;
    

    private void OnEnable()
    {
        
        showInventory.action.performed += InputShow;
        showInventory.action.canceled += InputShow;
        GameManager.Instance.OnPlayerInit += Init;
    }

    private void Init(PlayerStatus playerStatus)
    {
        itemManager = playerStatus.itemManager;
        itemManager.OnItemAdded += AddItem;
    }

    private void OnDisable()
    {
        showInventory.action.performed -= InputShow;
        showInventory.action.canceled -= InputShow;
    }


    private void InputShow(InputAction.CallbackContext context)
    {
        bool on = context.ReadValueAsButton();
        inventoryMenu.gameObject.SetActive(on);
        statsMenu.gameObject.SetActive(on);
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
        displayItem.SetImage(item.spriteRenderer.sprite);
        displayItem.SetBorder(item.GetBorderSprite(), item.GetBorderColor());
        displayItem.gameObject.name = item.name;
    }
}
