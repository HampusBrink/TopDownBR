using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private Dictionary<string, float> _floatDict = new Dictionary<string, float>();
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
            if (p.parameterValue is float value)
            {
                if(!_floatDict.ContainsKey(p.parameterName))
                    _floatDict[p.parameterName] = 0;
                _floatDict[p.parameterName] += value;
            }
        }
        OnItemAdded.Invoke(item);
    }

    public float GetFloat(string parameterName)
    {
        if (!_floatDict.ContainsKey(parameterName))
            return 0;

        return _floatDict[parameterName];
    }
}
