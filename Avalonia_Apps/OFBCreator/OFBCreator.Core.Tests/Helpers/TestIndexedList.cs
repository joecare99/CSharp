using System.Collections;
using GenInterfaces.Interfaces;

namespace OFBCreator.Core.Tests.Helpers;

/// <summary>
/// Concrete test double for IIndexedList&lt;T&gt; wrapping a real list.
/// Also implements IList&lt;T?&gt; for compatibility with GEDCOM interfaces.
/// </summary>
public class TestIndexedList<T> : IIndexedList<T?> where T : class
{
    private readonly List<T?> _inner = new();

    public IEnumerator<T?> GetEnumerator() => _inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
    public int Count => _inner.Count;
    public bool IsReadOnly => false;

    // IList<T?> explicit interface implementation
    T? IList<T?>.this[int index] { get => _inner[index]; set => _inner[index] = value; }
    int IList<T?>.IndexOf(T? item) => _inner.IndexOf(item);
    void IList<T?>.Insert(int index, T? item) => _inner.Insert(index, item);
    void IList<T?>.RemoveAt(int index) => _inner.RemoveAt(index);

    // IIndexedList<T?> explicit member
    T? IIndexedList<T?>.this[object index] { get => _inner[Convert.ToInt32(index)]; set => _inner[Convert.ToInt32(index)] = value; }

    // IIndexedList<T> methods
    public new object IndexOf(T item) => _inner.IndexOf(item)!;
    public void Insert(object index, T item) => _inner.Insert(Convert.ToInt32(index), item);
    public void RemoveAt(object index) => _inner.RemoveAt(Convert.ToInt32(index));
    public void Add(T item, object index) => _inner.Insert(Convert.ToInt32(index), item);

    // ICollection<T> additional methods
    public void Add(T? item) => _inner.Add(item);
    public void Clear() => _inner.Clear();
    public bool Contains(T? item) => _inner.Contains(item);
    public void CopyTo(T?[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);
    public bool Remove(T? item) => _inner.Remove(item);

    public int Add(T? item1, T? item2)
    {
        var idx = _inner.Count;
        _inner.Add(item1);
        _inner.Add(item2);
        return idx;
    }
}
