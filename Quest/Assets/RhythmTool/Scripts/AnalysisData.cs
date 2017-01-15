using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

/// <summary>
/// Stores the results of an analysis in readonly collections
/// </summary>
[System.Serializable]
public class AnalysisData {

    public string name { get; private set; }

    public ReadOnlyCollection<float> magnitude { get; private set; }
    public ReadOnlyCollection<float> flux { get; private set; }
    public ReadOnlyCollection<float> magnitudeSmooth { get; private set; }
    public ReadOnlyCollection<float> magnitudeAvg { get; private set; }

    public ReadOnlyDictionary<int, Onset> onsets { get; private set; }
    
    public AnalysisData(string name, List<float> magnitude, List<float> flux, List<float> magnitudeSmooth, List<float> magnitudeAvg, Dictionary<int, Onset> onsets)
    {
        this.name = name;
        this.magnitude = magnitude.AsReadOnly();
        this.flux = flux.AsReadOnly();
        this.magnitudeSmooth = magnitudeSmooth.AsReadOnly();
        this.magnitudeAvg = magnitudeAvg.AsReadOnly();
        this.onsets = new ReadOnlyDictionary<int, Onset>(onsets);
    }

    /// <summary>
    /// Returns an onset. Onset will be 0 if there is none detected for this index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Onset GetOnset(int index)
    {
        Onset o;

        onsets.TryGetValue(index, out o);

        return o;
    }
}

[Serializable]
public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly IDictionary<TKey, TValue> _dictionary;

    public ReadOnlyDictionary()
    {
        _dictionary = new Dictionary<TKey, TValue>();
    }

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    #region IDictionary<TKey,TValue> Members

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        throw ReadOnlyException();
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public ICollection<TKey> Keys
    {
        get { return _dictionary.Keys; }
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        throw ReadOnlyException();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public ICollection<TValue> Values
    {
        get { return _dictionary.Values; }
    }

    public TValue this[TKey key]
    {
        get
        {
            return _dictionary[key];
        }
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get
        {
            return this[key];
        }
        set
        {
            throw ReadOnlyException();
        }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        throw ReadOnlyException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        throw ReadOnlyException();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _dictionary.CopyTo(array, arrayIndex);
    }

    public int Count
    {
        get { return _dictionary.Count; }
    }

    public bool IsReadOnly
    {
        get { return true; }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        throw ReadOnlyException();
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    private static Exception ReadOnlyException()
    {
        return new NotSupportedException("This dictionary is read-only");
    }
}
