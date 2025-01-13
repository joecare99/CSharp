using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Model
{
    public class CNB_Family : CUsesRecordSet<int>, INB_Family
    {
        private Func<IRecordset> _value;

        public CNB_Family(Func<IRecordset> value)
        {
            _value = value;
        }
        protected override string __keyIndex => "Fam";

        protected override IRecordset _db_Table => _value();

        public override IRecordset? Seek(int key, out bool xBreak)
        {
            var NB_FamilyTable = _db_Table;
            NB_FamilyTable.Index = __keyIndex;
            NB_FamilyTable.Seek("=", key);
            xBreak = NB_FamilyTable.NoMatch;
            return xBreak ? null : NB_FamilyTable;
        }

        protected override int GetID(IRecordset recordset)
        {
            return recordset.Fields["Fam"].AsInt();
        }

        public void Append(int famInArb, bool xAppenWitt = true)
        {
            var NB_FamilyTable = _db_Table;
            NB_FamilyTable.AddNew();
            NB_FamilyTable.Fields["Fam"].Value = famInArb;
            NB_FamilyTable.Update();
        }
    }
}
