using BaseLib.Helper;
using GenFree.Interfaces.DB;
using GenFree.Model.Data;
using System;

namespace GenFree.Data
{
    public abstract class CRSDataInt<T> : CRSData<T, int> where T : Enum
    {
        protected int _ID;

        protected CRSDataInt(IRecordset db_Table, bool xNoInit= false) : base(db_Table,xNoInit){ }

        public override int ID => _ID;
        public override void NewID()
        {
            _db_Table.Index =nameof(_keyIndex) ;
            _db_Table.MoveLast();
            ReadID(_db_Table);
            _ID++;
            _changedPropsList.Add((T)(object)0);
        }
    }
}