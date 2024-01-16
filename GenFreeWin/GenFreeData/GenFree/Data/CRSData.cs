using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Data
{
    public abstract class CRSData<T, T2> : CData<T>, IHasIRecordset, IHasID<T2>
        where T : Enum
    {
        protected readonly IRecordset _db_Table;

        public abstract T2 ID { get; }

        public CRSData(IRecordset db_Table)
        {
            _db_Table = db_Table;
            FillData(db_Table);
        }

        protected abstract IRecordset? Seek(T2 iD);
        public void Delete()
        {
            Seek(ID)?.Delete();
        }

        public abstract void FillData(IRecordset dB_Table);
        public abstract void SetDBValue(IRecordset dB_Table, string[]? asProps);
    }
}