
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Model;
using System;
using System.Collections.Generic;

namespace GenFree.Model
{
    public abstract class CUsesIndexedRSet<T, T2, T3, T4> : CUsesRecordSet<T>, IHasRSIndex1<T2, T3> where T2 : Enum where T3 : Enum where T4 : IHasID<T>
    {
        public abstract T3 GetIndex1Field(T2 eIndex);
        protected abstract T4 GetData(IRecordset rs);

        public bool Exists(T2 eIndex, object oIndexVal)
            => Seek(eIndex, oIndexVal) != null;

        public IEnumerable<T4> ReadAll(T2 eIndex, object oIndexVal)
        {
            var rs = Seek(eIndex, oIndexVal);
            var eIdxFld = GetIndex1Field(eIndex);
            while (rs?.NoMatch == false
                  && !rs.EOF
                  && oIndexVal.Equals(rs.Fields[$"{eIdxFld}"].Value))
            {
                yield return GetData(rs);
                rs.MoveNext();
            }
        }

        public IRecordset? Seek(T2 eIndex, object oValue)
        {
            var dB_EventTable = _db_Table;
            dB_EventTable.Index = $"{eIndex}";
            dB_EventTable.Seek("=", oValue);
            return dB_EventTable.NoMatch ? null : dB_EventTable;
        }

        public bool ReadData(T2 eIndex, object iValue, out T4? cData)
        {
            var dB_EventTable = Seek(eIndex, iValue);
            cData = (dB_EventTable == null) ? default : GetData(dB_EventTable);
            return !(cData != null);
        }

        public void ForeachDo(T2 eIndex, T3 eIndexField, object oIndexVal, Func<T4, bool> func)
        {
            var rs = Seek(eIndex, oIndexVal);
            while (rs?.EOF == false
                && !rs.NoMatch
                && oIndexVal.Equals(rs.Fields[$"{eIndexField}"].Value)
                && func(GetData(rs)))
            {
                rs.MoveNext();
            }
        }

    }
}