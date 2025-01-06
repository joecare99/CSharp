using System.Collections.Generic;

namespace GenFree2Base.Interfaces;

public interface IIndexedList<T> : IList<T> 
{
    T this[object index] { get; set; }
    new object IndexOf(T item);
    void Insert(object index, T item);
    void RemoveAt(object index);
}