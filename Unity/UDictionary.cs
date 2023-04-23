using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//A dictionary class that can be serialized by Unity

/// Authors: TCGM
/// Assistants: CGPT4, CGPT3
/// <summary>
/// A dictionary class that can be serialized by Unity
/// </summary>
/// Unity Location: Goes in normal code folders
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>

[Serializable]
public class UDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    // The list of keys used for serialization
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    // The list of values used for serialization
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    // The dictionary used to store the actual key-value pairs
    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    // A property that exposes the underlying dictionary
    public Dictionary<TKey, TValue> Dictionary
    {
        get { return dictionary; }
    }

    // A property that exposes the keys in the dictionary
    public List<TKey> Keys
    {
        get { return keys; }
    }

    // A property that exposes the values in the dictionary
    public List<TValue> Values
    {
        get { return values; }
    }

    // Implement the indexer to get/set values via array referencing
    public TValue this[TKey key]
    {
        get
        {
            return dictionary[key];
        }
        set
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                int index = keys.IndexOf(key);
                values[index] = value;
            }
            else
            {
                dictionary.Add(key, value);
                keys.Add(key);
                values.Add(value);
            }
        }
    }

    // Adds a key-value pair to the dictionary
    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
        keys.Add(key);
        values.Add(value);
    }

    // Removes a key from the dictionary
    public void Remove(TKey key)
    {
        TValue val = dictionary[key];
        dictionary.Remove(key);
        keys.Remove(key);
        values.Remove(val);
    }

    // Removes all key-value pairs from the dictionary
    public void Clear()
    {
        dictionary.Clear();
        keys.Clear();
        values.Clear();
    }

    // This method is called by Unity before serialization occurs
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        // Iterate over the key-value pairs in the dictionary and add them to the lists
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    // This method is called by Unity after deserialization occurs
    public void OnAfterDeserialize()
    {
        dictionary.Clear();

        // Iterate over the keys and values in the lists and add them to the dictionary
        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            TKey key = keys[i];
            TValue value = values[i];
            dictionary.Add(key, value);
        }
    }

    // Tries to get the value associated with the specified key
    // Returns true if the key was found and the value was returned successfully, false otherwise
    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    //Joins an IDictionary into this UDictionary.
    public void Union(IDictionary<TKey, TValue> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException("UDictionary: Passed IDictionary was invalid.");
        }

        foreach (var kvp in other)
        {
            if (!dictionary.ContainsKey(kvp.Key))
            {
                dictionary.Add(kvp.Key, kvp.Value);
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }
    }

    //Joins another UDictionary into this UDictionary and returns the result.
    public UDictionary<TKey, TValue> Union(UDictionary<TKey, TValue> other)
    {
        var result = new UDictionary<TKey, TValue>();

        // Add all items from the current dictionary to the result
        foreach (var kvp in dictionary)
        {
            result.Add(kvp.Key, kvp.Value);
        }

        // Add all items from the other dictionary that are not already in the result
        foreach (var kvp in other.Dictionary)
        {
            if (!result.ContainsKey(kvp.Key))
            {
                result.Add(kvp.Key, kvp.Value);
            }
        }

        return result;
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public bool ContainsValue(TValue value)
    {
        return dictionary.ContainsValue(value);
    }

    public IDictionary<TKey, TValue> ToDictionary()
    {
        return dictionary;
    }

    public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(UDictionary<TKey, TValue> uDictionary)
    {
        return uDictionary.Dictionary.ToDictionary(x => x.Key, x => x.Value);
    }

    public UDictionary<TKey, TValue> ToUDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        var uDictionary = new UDictionary<TKey, TValue>();
        foreach (var kvp in dictionary)
        {
            uDictionary.Add(kvp.Key, kvp.Value);
        }
        return uDictionary;
    }

    //TODO
    /*public UDictionary<TKey, TValue> ToDictionary<TKey2, TValue2>(Func<KeyValuePair<TKey, TValue>, TKey2> keySelector,
        Func<KeyValuePair<TKey, TValue>, TValue2> valueSelector)
    {
        var dict = new UDictionary<TKey2, TValue2>();

        foreach (var kvp in dictionary)
        {
            dict.Add(keySelector(kvp), valueSelector(kvp));
        }

        return dict;
    }
    public static UDictionary<TKey, TValue> ConvertTypes<TKey, TValue, TKey2, TValue2>(
    UDictionary<TKey2, TValue2> dictionary,
    Func<TKey2, TKey> keyConverter,
    Func<TValue2, TValue> valueConverter)
    {
        var result = new UDictionary<TKey, TValue>();
        foreach (var pair in dictionary)
        {
            if (pair.Key is TKey2 && pair.Value is TValue2)
            {
                var key = keyConverter((TKey2)pair.Key);
                var value = valueConverter((TValue2)pair.Value);
                result.Add(key, value);
            }
            else
            {
                throw new ArgumentException("Dictionary contains elements of incompatible types.");
            }
        }
        return result;
    }*/
}
