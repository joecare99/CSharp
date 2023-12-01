//using DAO;
using GenFree.Helper;
using GenFree.Interfaces.DB;

//using DAO;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Data
{
    public abstract class CUsesRecordSet<T> : IUsesRecordset<T>
    {
        public int Count => _db_Table.RecordCount;
        public T MaxID => GetMaxID();

        protected abstract string _keyIndex { get; }
        protected abstract IRecordset _db_Table { get; }
        protected abstract T GetID(IRecordset recordset);
        public abstract IRecordset? Seek(T key, out bool xBreak);

        public bool Delete(T key) => Seek(key, out var x).SetRet(rs => rs?.Delete(), x);
        public bool Exists(T key)=>Seek(key) != null;
       
        public void ForEachDo(Action<IRecordset> action)
        {
            IRecordset dB_PlaceTable = _db_Table;
            dB_PlaceTable.Index = _keyIndex;
            dB_PlaceTable.MoveFirst();
            while (!dB_PlaceTable.EOF)
            {
                try { action?.Invoke(dB_PlaceTable); } catch { };
                dB_PlaceTable.MoveNext();
            }
        }

        public  IRecordset? Seek(T Key) => Seek(Key, out _);
        private T GetMaxID()
        {
            IRecordset dB_PlaceTable = _db_Table;
            dB_PlaceTable.Index = _keyIndex;
            dB_PlaceTable.MoveLast();
            return GetID(dB_PlaceTable);
        }
    }
}