using BaseGenClasses.Helper.Interfaces;
using GenInterfaces.Interfaces;
using System;

namespace BaseGenClasses.Helper;

public class GenILBuilder : IGenILBuilder
{
    public IIndexedList<T> NewList<T, T2>(Func<T,T2> getIdx) where T : class where T2 : notnull
    {
        return new IndexedList<T, T2>(getIdx);
    }
}