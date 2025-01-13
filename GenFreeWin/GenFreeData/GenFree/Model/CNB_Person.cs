using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Model
{
    public class CNB_Person : CUsesRecordSet<int>, INB_Person
    {
        private Func<IRecordset> _value;

        private Action<int> _aApeendPaten;
        public CNB_Person(Func<IRecordset> value, Action<int> appendPaten)
        {
            _value = value;
            _aApeendPaten = appendPaten;
        }
        protected override string __keyIndex => "Per";
        protected override IRecordset _db_Table => _value();
        protected override int GetID(IRecordset recordset)
        {
            return recordset.Fields["Person"].AsInt();
        }

        public override IRecordset? Seek(int key, out bool xBreak)
        {
            var NB_PersonTable = _db_Table;
            NB_PersonTable.Index = __keyIndex;
            NB_PersonTable.Seek("=", key);
            xBreak = NB_PersonTable.NoMatch;
            return xBreak ? null : NB_PersonTable;
        }

        public void Append(int persInArb, bool xAppenWitt = true)
        {
            var NB_PersonTable = _db_Table;
            NB_PersonTable.AddNew();
            NB_PersonTable.Fields["Person"].Value = persInArb;
            NB_PersonTable.Update();
            if (xAppenWitt)
            {
                _aApeendPaten(persInArb);
            }
        }
    }
}
