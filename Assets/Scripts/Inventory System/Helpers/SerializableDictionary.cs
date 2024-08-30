using System;
using System.Collections.Generic;
using UnityEngine;

//https://github.com/azixMcAze/Unity-SerializableDictionary
[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public Dictionary<TKey, TValue> ToDictionary() => dictionary;

    public void OnBeforeSerialize()
    {
       
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear(); 
      
        if (keys.Count != values.Count)
        {
            if (keys.Count > values.Count)
            {
                keys.RemoveRange(values.Count, keys.Count - values.Count);
            }
            else if (values.Count > keys.Count)
            {
                values.RemoveRange(keys.Count, values.Count - keys.Count);
            }
        }

        // Populate the dictionary with the synchronized key-value pairs
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public void Add(TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            keys.Add(key);
            values.Add(value);
        }
        else
        {
            Debug.LogWarning($"Key '{key}' already exists in the dictionary.");
        }
    }

    public bool Remove(TKey key)
    {
        if (dictionary.ContainsKey(key))
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
            return dictionary.Remove(key);
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public int Count => dictionary.Count;

    public void Clear()
    {
        dictionary.Clear();
        keys.Clear();
        values.Clear();
    }
}
