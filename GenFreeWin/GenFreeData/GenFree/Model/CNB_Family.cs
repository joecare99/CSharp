using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Model
{
    public class CNB_Family : CUsesRecordSet<int>, INB_Family
    {
        private Func<IRecordset> _value;

        public CNB_Family(Func<IRecordset> value)
        {
            _value = value;
        }
        protected override string _keyIndex => "Fam";

        protected override IRecordset _db_Table => _value();

        public override IRecordset? Seek(int key, out bool xBreak)
        {
            var NB_FamilyTable = _db_Table;
            NB_FamilyTable.Index = _keyIndex;
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
