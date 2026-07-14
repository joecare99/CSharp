using BaseLib.Helper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GenFreeWin.ViewModels;

public class BoolProxy<T2, T>(Func<IList<T>> getter, Func<T, bool> conv, Func<bool, T>? reconv = null) : IList<bool> where T2 : Enum
{
    public bool this[T2 idx] { get => this[idx.AsInt()]; set => this[idx.AsInt()] = value; }

    public bool this[int index] { get => conv(getter()[index]); set => getter()[index] = reconv!.Invoke(value); }

    public int Count => getter().Count;

    public bool IsReadOnly => getter().IsReadOnly;

    public void Add(bool item)
    {
        getter().Add(reconv(item));
    }

    public void Clear()
    {
        getter().Clear();
    }

    public bool Contains(bool item)
    {
        return true;
    }

    public void CopyTo(bool[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<bool> GetEnumerator()
    {
        foreach (var i in getter())
            yield return conv(i);
    }

    public int IndexOf(bool item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, bool item)
    {
        getter().Insert(index, reconv(item));
    }

    public bool Remove(bool item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        getter().RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
