//using DAO;
using GenFree.Helper;
using GenFree.Interfaces.DB;

//using DAO;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Model
{
    public abstract class CUsesRecordSet<T> : IUsesRecordset<T>
    {
        public int Count => _db_Table?.RecordCount ?? 0;
        public T MaxID => GetMaxID();

        protected abstract string __keyIndex { get; }
        protected abstract IRecordset _db_Table { get; }
        protected abstract T GetID(IRecordset recordset);
        public abstract IRecordset? Seek(T key, out bool xBreak);

        public bool Delete(T key) => Seek(key, out var x).SetRet(rs => rs?.Delete(), !x);
        public bool Exists(T key) => Seek(key) != null;

        public void ForEachDo(Action<IRecordset> action)
        {
            IRecordset dB_Table = _db_Table;
            dB_Table.Index = __keyIndex;
            dB_Table.MoveFirst();
            while (!dB_Table.EOF)
            {
                try { action?.Invoke(dB_Table); } catch { };
                dB_Table.MoveNext();
            }
        }

        public IRecordset? Seek(T Key) => Seek(Key, out _);
        private T GetMaxID()
        {
            IRecordset dB_Table = _db_Table;
            dB_Table.Index = __keyIndex;
            dB_Table.MoveLast();
            return GetID(dB_Table);
        }
    }
}