using System;
using System.Collections;
using System.Collections.Generic;
using GenInterfaces.Interfaces;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomIndexedList<T> : IIndexedList<T> where T : class
{
    private readonly List<T?> _items = new();
    private readonly Dictionary<object, T> _indexedItems = new();

    public T? this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public T? this[object index]
    {
        get => _indexedItems.TryGetValue(index, out var item) ? item : null;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _indexedItems[index] = value;
            if (!_items.Contains(value))
                _items.Add(value);
        }
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public void Add(T? item)
    {
        _items.Add(item);
    }

    public void Add(T item, object index)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(index);
        _indexedItems.Add(index, item);
        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
        _indexedItems.Clear();
    }

    public bool Contains(T? item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(T?[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T?> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public int IndexOf(T? item)
    {
        return _items.IndexOf(item);
    }

    object? IIndexedList<T>.IndexOf(T item)
    {
        foreach (var pair in _indexedItems)
            if (ReferenceEquals(pair.Value, item))
                return pair.Key;

        return null;
    }

    public void Insert(int index, T? item)
    {
        _items.Insert(index, item);
    }

    public void Insert(object index, T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(index);
        _indexedItems.Add(index, item);
        _items.Add(item);
    }

    public bool Remove(T? item)
    {
        return _items.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    public void RemoveAt(object index)
    {
        if (!_indexedItems.Remove(index, out var item))
            throw new KeyNotFoundException($"No item is registered for index '{index}'.");

        _items.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
