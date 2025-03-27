using System.Collections.Generic;

namespace GenInterfaces.Interfaces;

public interface IIndexedList<T> : IList<T> where T : class
{
    T this[object index] { get; set; }
    new object IndexOf(T item);
    void Insert(object index, T item);
    void RemoveAt(object index);
    void Add(T Item, object index);
}