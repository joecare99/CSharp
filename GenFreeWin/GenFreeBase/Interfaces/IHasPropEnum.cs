//using DAO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GenFree.Interfaces
{
    public interface IHasPropEnum<T> where T : Enum
    {
        IReadOnlyList<T> ChangedProps { get; }
        Type GetPropType(T prop);
        object? GetPropValue(T prop);
        T2? GetPropValue<T2>(T prop);
        void SetPropValue(T prop, object value);
        void ClearChangedProps();
        void AddChangedProp(T prop);
    }
}