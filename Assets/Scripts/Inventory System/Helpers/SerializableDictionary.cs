using System;
using System.Collections.Generic;
using UnityEngine;

// https://github.com/azixMcAze/Unity-SerializableDictionary
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
        // Czyszczenie kluicze przed synchronziacja
        keys.Clear();
        values.Clear();

        // dicitoanry to list konwersja
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear(); // Czyszczenie słownika przed deserializacją

        // Sprawdzanie i naprawa ewentualnych niezgodności między listami kluczy i wartości
        if (keys.Count != values.Count)
        {
            Debug.LogError($"Błąd serializacji: Po deserializacji jest {keys.Count} kluczy i {values.Count} wartości. Listy muszą mieć tę samą długość.");

            // usuwam nadmiarowe elementy ??? 
            if (keys.Count > values.Count)
            {
                keys.RemoveRange(values.Count, keys.Count - values.Count);
            }
            else if (values.Count > keys.Count)
            {
                values.RemoveRange(keys.Count, values.Count - keys.Count);
            }
        }

        // proboje odtworzyc dicitonary z zsynchronizowanych list kluczy i wartości
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    public TValue this[TKey key]
    {
        get => dictionary[key]; // 
        set => dictionary[key] = value; // Ustawiam wartości dla danego klucza
    }

    public void Add(TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value); // Dodawanie pary klucz-wartość do dic
            keys.Add(key); // Dodawanie klucza do listy kluczy
            values.Add(value); // Dodawanie wartości do listy wartości
        }
        else
        {
            Debug.LogWarning($"Klucz '{key}' już istnieje w dictionary.");
        }
    }

    public bool Remove(TKey key)
    {
        if (dictionary.ContainsKey(key))
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index); // Usuwanie klucza z listy kluczy
                values.RemoveAt(index); // Usuwanie wartości z listy wartości
            }
            return dictionary.Remove(key); // Usuwanie pary klucz-wartośćz dicitonary
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key); // Sprawdzam czy zawiera adny klucz
    }

    public int Count => dictionary.Count; // Zwracanie liczby elementów 

    public void Clear()
    {
        // cleanup wartosci kluiczy i dictionary
        dictionary.Clear(); 
        keys.Clear(); 
        values.Clear(); 
    }
}
