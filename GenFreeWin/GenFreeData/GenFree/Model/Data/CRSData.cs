using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Models.Data
{
    public abstract class CRSData<T, T2> : CRSDataC<T, T2>
     where T : Enum
        where T2 : struct
    {
        protected abstract Enum _keyIndex { get; }
        protected CRSData(IRecordset db_Table, bool xNoInit =false) : base(db_Table,xNoInit)
        {
        }

        protected override IRecordset? Seek(T2 iD)
        {
            _db_Table.Index = _keyIndex.ToString();
            _db_Table.Seek("=", iD);
            return _db_Table.NoMatch?null:_db_Table;
        }

    }

    public abstract class CRSDataC<T, T2> : CData<T>, IHasIRecordset, IHasID<T2>
 where T : Enum
    {
        protected readonly IRecordset _db_Table;

        public abstract T2 ID { get; }

        public CRSDataC(IRecordset db_Table, bool xNoInit=false)
        {
            _db_Table = db_Table;
            if (!xNoInit) FillData(db_Table);
        }
        protected abstract IRecordset? Seek(T2 iD);

        public void Delete()
        {
            Seek(ID)?.Delete();
        }

        public abstract void FillData(IRecordset dB_Table);
        public abstract void ReadID(IRecordset dB_Table);
        public abstract void SetDBValues(IRecordset dB_Table, Enum[]? asProps);

        public virtual void NewID() { }
    }
}