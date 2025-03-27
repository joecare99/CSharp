using GenInterfaces.Interfaces;
using System;

namespace BaseGenClasses.Helper.Interfaces;

public interface IGenILBuilder
{
    IIndexedList<T> NewList<T, T2>(Func<T,T2> getIdx) where T : class where T2: notnull;
}