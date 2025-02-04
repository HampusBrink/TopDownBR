using System.Collections.Generic;
using FishNet.Object;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemContainer : NetworkBehaviour
{
    // Server side loot table/same for everyone
    // On open client asks server for loot table, if it doesn't exist, server creates one.

    private ItemManager _giveTo = null;

    public List<Item> items;
    public ContainerItem containerItemPrefab;
    public Transform displayParent;
    public GameObject itemView;
    
    private List<ContainerItem> _savedItems = new List<ContainerItem>();
    public List<Item> _tempItems;
    private bool _isCurrentlyOpen = false;
   
    
    
    public void OpenContainer(PlayerStatus playerStatus)
    {
        if (_isCurrentlyOpen)
            return;

        _giveTo = playerStatus.itemManager;
        _savedItems = GetItems();
        DisplayItems();
        _isCurrentlyOpen = true;
    }

    public void CloseContainer()
    {
        _isCurrentlyOpen = false;
    }

    private void GiveItem(Item i)
    {
        _giveTo.TakeItem(i);
        itemView.SetActive(false);
        SRPC_SetChestOpened(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SRPC_SetChestOpened(bool isOpen)
    {
        ORPC_SetChestOpened(isOpen);
    }

    [ObserversRpc]
    private void ORPC_SetChestOpened(bool isOpen)
    {
        SetChestOpened(isOpen);
    }

    private List<ContainerItem> GetItems() // switch to other class
    {
        if (_savedItems.Count != 0)
            return _savedItems;
        
        List<ContainerItem> items = new List<ContainerItem>();
        _tempItems = new List<Item>(this.items);
        
        for (int i = 0; i < 3; i++)
        {
            items.Add(CreateContainerItem());
        }

        return items;
    }

    private ContainerItem CreateContainerItem()
    {
        ContainerItem containerItem = Instantiate(containerItemPrefab, displayParent);
        containerItem.transform.SetParent(displayParent);
        containerItem.Init(PullRandomItem(_tempItems));
        containerItem.OnPick += GiveItem;
        
        return containerItem;
    }

    private Item PullRandomItem(List<Item> items)
    {
        var totalWeight = 0;
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }

        var randomWeight = Random.Range(0f, totalWeight);
        
        var cumulativeWeight = 0;
        foreach (var item in items)
        {
            cumulativeWeight += item.weight;
            if (randomWeight <= cumulativeWeight)
            {
                items.Remove(item);
                return item;
            }
        }

        return null;
    }

    private void SetChestOpened(bool isOpen)
    {
        //TODO: add chest open and closed sprite
        gameObject.SetActive(!isOpen);
        _isCurrentlyOpen = isOpen;
    }

    private void DisplayItems()
    {
        itemView.SetActive(true);
    }
}
