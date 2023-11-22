﻿using System.Collections;
using System.Collections.Generic;

namespace WinAhnenCls.Model.GenBase
{
    public interface IGenList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        T this[object index] { get; set; }
        object IndexOf(T item);
        void Insert(object index, T item);
        void RemoveAt(object index);
    }
}