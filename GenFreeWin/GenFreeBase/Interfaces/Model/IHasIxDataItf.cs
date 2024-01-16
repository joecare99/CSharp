//using DAO;

using System;

namespace GenFree.Interfaces.Model
{
    public interface IHasIxDataItf<T, T1, T2> : IHasDataItf<T1, T2>
        where T1 : IHasID<T2>, IHasIRecordset
        where T : Enum
    {
        System.Collections.Generic.IEnumerable<T1> ReadAll(T eIndex);
    }
}