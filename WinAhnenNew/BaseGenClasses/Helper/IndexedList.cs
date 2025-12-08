using GenInterfaces.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BaseGenClasses.Helper;

public class IndexedList<T,T2>(Func<T,T2> getIdx): WeakLinkList<T>, IIndexedList<T> where T : class where T2 : notnull
{
    private Dictionary<T2,int> _list = new();

    private Func<T, T2> _getIdx = getIdx;
    public T? this[object index] { get => _list.TryGetValue((T2)index,out var iix)?this[iix]:null ; set => throw new NotImplementedException(); }

    public override void Add(T? item)
    {
        if (item == null) return;
        base.Add(item);
        _list.Add(_getIdx(item), Count - 1);
    }

    public void Add(T Item, object index)
    {
         Add(Item);
        _list.Add((T2)index, Count-1);
    }

    public override void Clear()
    {
        base.Clear();
        _list.Clear();
    }

    public override bool Remove(T? item)
    {
        if (item == null) return false;
        if (_list.TryGetValue(_getIdx(item), out var i))
        {
            _list.Remove(_getIdx(item));
            base.RemoveAt(i);
            return true;
        }
        return base.Remove(item);
    }

    public override void Insert(int index, T? item)
    {
        if (item == null) return;
        base.Insert(index, item);
        _list.Add(_getIdx(item), index);
    }

    public void Insert(object index, T item)
    {
        base.Insert(_list[(T2)index], item);
    }

    public override void RemoveAt(int index)
    {
        T? t = this[index];
        if (t!=null) _list.Remove(_getIdx(t));
        base.RemoveAt(index);
    }

    public void RemoveAt(object index)
    {
        base.RemoveAt(_list[(T2)index]);
    }

    object? IIndexedList<T>.IndexOf(T item)
    {
        var i = base.IndexOf(item);
        return i == -1 ? null : _list.FirstOrDefault(x => x.Value == i).Key;
    }
}