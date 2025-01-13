//using DAO;

using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model
{
    public interface IHasIxDataItf<T, T1, T2> : IHasDataItf<T1, T2>, IUsesID<T2>
        where T1 : IHasID<T2>, IHasIRecordset
        where T : Enum
    {
        IEnumerable<T1> ReadAll(T eIndex);
        IEnumerable<T1> ReadAll(T eIndex, object oIndexVal);

    }
}