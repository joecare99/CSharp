using System;
using System.Collections.Generic;
using System.Linq;

namespace GenFree.Interfaces;

public interface IArrayProxy<T> : IEnumerable<T>
{
    T this[object Idx] { get; set; }

    Func<object, T?> getaction { get; }
    Func<IEnumerator<T>>? getenum { get; set; }
    Action<object, T>? setaction { get; }

}