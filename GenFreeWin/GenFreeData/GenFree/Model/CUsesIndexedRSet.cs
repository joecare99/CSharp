
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Model;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GenFree.Model
{
    public abstract class CUsesIndexedRSet<T, T2, T3, T4> : 
        CUsesRecordSet<T>,
        IHasIxDataItf<T2, T4, T>,
        IHasRSIndex1<T2, T3>
        where T2 : Enum where T3 : Enum where T4 : IHasID<T>, IHasIRecordset
    {
        public abstract T3 GetIndex1Field(T2 eIndex);
        protected abstract T4 GetData(IRecordset rs);

        protected abstract T2 _keyIndex { get; }
        protected override string __keyIndex => _keyIndex.AsString();

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

        public IEnumerable<T4> ReadAll()
            => ReadAllDataDB(_keyIndex, (r)=>r.MoveFirst(),(c)=>false);

        public IEnumerable<T4> ReadAll(T2 eIndex)
            => ReadAllDataDB(eIndex, (rs) => rs.MoveFirst(), (e) => false);


        public override IRecordset? Seek(T tValue, out bool xBreak)
        {
            if (tValue is not ValueType vVal) throw new NotImplementedException();
            var dB_Table = _db_Table;
            dB_Table.Index = $"{_keyIndex}";
            dB_Table.Seek("=", vVal);
            xBreak = dB_Table.NoMatch;
            return xBreak ? null : dB_Table;
        }

        public IRecordset? Seek(T2 eIndex, object oValue)
            => Seek(eIndex, oValue, out _);

        public IRecordset? Seek(T2 eIndex, object oValue, out bool xBreak)
        {
            var dB_EventTable = _db_Table;
            dB_EventTable.Index = $"{eIndex}";
            dB_EventTable.Seek("=", oValue);
            return (xBreak = dB_EventTable.NoMatch) ? null : dB_EventTable;
        }

        public bool ReadData(T2 eIndex, object iValue, out T4? cData)
        {
            var dB_EventTable = Seek(eIndex, iValue);
            cData = (dB_EventTable == null) ? default : GetData(dB_EventTable);
            return (cData != null);
        }

        public void ForEachDo(T2 eIndex, T3 eIndexField, object oIndexVal, Func<T4, bool> func)
        {
            var rs = Seek(eIndex, oIndexVal);
            while (rs?.EOF == false
                && !rs.NoMatch
                && oIndexVal.Equals(rs.Fields[$"{eIndexField}"].Value)
                && lFunc(GetData(rs)))
            {
                rs.MoveNext();
            }

            bool lFunc(T4 cDt)
            {
                try
                {
                    return func?.Invoke(cDt) ?? false;
                }
                catch
                {
                    return true;
                }
            }
        }

        protected IEnumerable<T4> ReadAllDataDB(T2 Idx, Action<IRecordset> SeekAct, Predicate<T4> StopPred)
        {
            var db_Table = _db_Table;
            db_Table.Index = Idx.AsString();
            SeekAct(db_Table);
            while (!db_Table.NoMatch
                && !db_Table.EOF)
            {
                var Link = GetData(db_Table);
                if (StopPred(Link))
                    break;
                yield return Link;
                db_Table.MoveNext();
            }
            yield break;
        }

        public bool ReadData(T key, out T4? data)
        {
            var dB_Table = Seek(key, out bool xBreak);
            data = xBreak ? default : GetData(dB_Table!);
            return !xBreak;
        }

        public void SetData(T key, T4 data, string[]? asProps = null)
        {
            var dB_Table = Seek(key);
            if (dB_Table != null)
            {
                dB_Table.Edit();
                data.SetDBValue(dB_Table, asProps);
                dB_Table.Update();
            }
        }
    }
}