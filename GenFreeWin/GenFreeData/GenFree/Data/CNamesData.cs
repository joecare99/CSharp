using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;

namespace GenFree.Data
{
    public class CNamesData : INamesData
    {
        public CNamesData(IRecordset? dB_NamesTable)
        {
        }

        public IReadOnlyList<ENamesProp> ChangedProps => throw new NotImplementedException();

        public (int, ETextKennz, int) ID => throw new NotImplementedException();

        public void AddChangedProp(ENamesProp prop)
        {
            throw new NotImplementedException();
        }

        public void ClearChangedProps()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void FillData(IRecordset dB_Table)
        {
            throw new NotImplementedException();
        }

        public Type GetPropType(ENamesProp prop)
        {
            throw new NotImplementedException();
        }

        public object GetPropValue(ENamesProp prop)
        {
            throw new NotImplementedException();
        }

        public T2 GetPropValue<T2>(ENamesProp prop)
        {
            throw new NotImplementedException();
        }

        public void SetDBValue(IRecordset dB_NamesTable, string[]? asProps)
        {
            throw new NotImplementedException();
        }

        public void SetPropValue(ENamesProp prop, object value)
        {
            throw new NotImplementedException();
        }
    }
}