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
        SRPC_DestroyItem();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SRPC_DestroyItem()
    {
        NetworkManager.ServerManager.Despawn(gameObject);
    }

    private List<ContainerItem> GetItems() // switch to other class
    {
        if (_savedItems.Count != 0)
            return _savedItems;
        
        List<ContainerItem> items = new List<ContainerItem>();
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
        containerItem.Init(PullRandomItem(items));
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

    private void DisplayItems()
    {
        itemView.SetActive(true);
    }
}
