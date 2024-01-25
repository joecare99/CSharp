using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Model
{
    public class COFB : CUsesIndexedRSet<(int, string, int), OFBIndex, OFBFields, IOFBData>, IOFB
    {
        private Func<IRecordset> _value;

        public COFB(Func<IRecordset> value)
        {
            _value = value;
        }

        protected override OFBIndex _keyIndex => OFBIndex.Indn;

        protected override IRecordset _db_Table => _value();

        public override OFBFields GetIndex1Field(OFBIndex eIndex)
        {
            return eIndex switch
            {
                OFBIndex.InDNr => OFBFields.PerNr,
                OFBIndex.IndNum => OFBFields.TextNr,
                _ => throw new ArgumentException()
            };
        }

        protected override IOFBData GetData(IRecordset rs)
        {
            return new COFBData(rs);
        }

        public override IRecordset? Seek((int, string, int) tValue, out bool xBreak)
        {
            var db_Table = _db_Table;
            db_Table.Index = $"{_keyIndex}";
            db_Table.Seek("=", tValue.Item1, tValue.Item2, tValue.Item3);
            xBreak = db_Table.NoMatch;
            return xBreak ? null : db_Table;
        }

        protected override (int, string, int) GetID(IRecordset recordset)
        {
            return (
                recordset.Fields[nameof(OFBFields.PerNr)].AsInt(),
                recordset.Fields[nameof(OFBFields.Kennz)].AsString(),
                recordset.Fields[nameof(OFBFields.TextNr)].AsInt());
        }
    }
}
