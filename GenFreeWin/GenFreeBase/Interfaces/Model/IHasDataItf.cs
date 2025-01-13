using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model
{
    public interface IHasDataItf<T,T2> : IUsesID<T2> where T : IHasID<T2>
    {
        bool ReadData(T2 key,out T? data);
        IEnumerable<T> ReadAll();
        void SetData(T2 key,T data, Enum[]? asProps = null);
    }
}
