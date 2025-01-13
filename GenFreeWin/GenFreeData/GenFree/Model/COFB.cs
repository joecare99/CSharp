using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;

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

        public bool TextExist(int TextNr)
        {
            IRecordset dB_OFBTable = _db_Table;
            dB_OFBTable.Index = OFBIndex.IndNum.AsFld();
            dB_OFBTable.Seek("=", TextNr);
            return !dB_OFBTable.NoMatch;
        }

        public bool Exists(OFBIndex index, int persInArb, string sKennz)
        {
            IRecordset dB_OFBTable = _db_Table;
            dB_OFBTable.Index = index.AsFld();
            dB_OFBTable.Seek("=", persInArb, sKennz);
            return !dB_OFBTable.NoMatch;
        }

        public void Update(string Kennz, int persInArb, int satz)
        {
            IRecordset dB_OFBTable = _db_Table;
            dB_OFBTable.Index = OFBIndex.Indn.AsFld();
            dB_OFBTable.Seek("=", persInArb, Kennz, satz);
            if (dB_OFBTable.NoMatch)
            {
                dB_OFBTable.AddNew();
                dB_OFBTable.Fields["PerNr"].Value = persInArb;
                dB_OFBTable.Fields["Kennz"].Value = Kennz;
                dB_OFBTable.Fields["TextNr"].Value = satz;
                dB_OFBTable.Update();
            }
        }

        public bool DeleteIndNr(int persInArb, string v)
        {
            SeekIndNr(persInArb, v, out var xB)?.Delete();
            return !xB;
        }

        public IRecordset? SeekIndNr(int persInArb, string v, out bool xB)
        {
            var db_Table = _db_Table;
            db_Table.Index = OFBIndex.InDNr.AsFld();
            db_Table.Seek("=", persInArb, v);
            return (xB = db_Table.NoMatch) ? null : db_Table;
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
