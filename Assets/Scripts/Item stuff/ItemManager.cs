using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private Dictionary<string, List<object>> _paramDict = new Dictionary<string, List<object>>();
    private List<Item> _items = new List<Item>();
    
    public Action<Item> OnItemAdded = delegate { };

    public List<Item> GetItems()
    {
        return _items;
    }
    
    public void TakeItem(Item item)
    {
        foreach (var p in item.parameters)
        {
            if(!_paramDict.ContainsKey(p.parameterName))
                _paramDict[p.parameterName] = new List<object>();
            _paramDict[p.parameterName].Add(p.parameterValue);
        }
        OnItemAdded.Invoke(item);
    }

    public float GetFloat(string parameterName)
    {
        if (!_paramDict.ContainsKey(parameterName))
            return 0;
        float f = 0;
        foreach (var p in _paramDict[parameterName])
        {
            Debug.Log((float)p);
            f += (float)p;
            
        }

        return f;
    }
}
