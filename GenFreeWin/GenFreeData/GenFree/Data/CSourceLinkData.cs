using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Helper;
using GenFree.Interfaces;
using System.Collections.Generic;
using System;

namespace GenFree.Data
{
    public class CSourceLinkData : CRSData<ESourceLinkProp, (int, EEventArt, int)>, ISourceLinkData
    {
        public CSourceLinkData() : base(null)
        {
        }

        public CSourceLinkData(IRecordset recordset) : base(recordset)
        {
        }

        public int iLinkType { get; internal set; }
        public EEventArt eArt { get; internal set; }
        public int iPersNr { get; internal set; }
        public int iLfdNr { get; internal set; }
        public string sKom { get; internal set; }
        public string sOrig { get; internal set; }
        public string sAus { get; internal set; }
        public string sField3 { get; internal set; }
        public int iQuNr { get; internal set; }

        public override (int, EEventArt, int) ID =>
            (iLinkType, eArt, iPersNr);

        public override void FillData(IRecordset db_Table)
        {
            eArt = db_Table.Fields["Art"].AsEnum<EEventArt>();
            iLinkType = db_Table.Fields[0].AsInt();
            iPersNr = db_Table.Fields[1].AsInt();
            iLfdNr = db_Table.Fields["LfNr"].AsInt();
            iQuNr = db_Table.Fields["3"].AsInt();
            sField3 = db_Table.Fields[3].AsString();
            sAus = db_Table.Fields["Aus"].AsString();
            sOrig = db_Table.Fields["Orig"].AsString();
            sKom = db_Table.Fields["Kom"].AsString();
        }

        public override Type GetPropType(ESourceLinkProp prop)
        {
            throw new NotImplementedException();
        }

        public override object GetPropValue(ESourceLinkProp prop)
        {
            throw new NotImplementedException();
        }

        protected override IRecordset? Seek((int, EEventArt, int) iD)
        {
            throw new NotImplementedException();
        }

        public override void SetDBValue(IRecordset dB_Table, string[]? asProps)
        {
            throw new NotImplementedException();
        }

        public override void SetPropValue(ESourceLinkProp prop, object value)
        {
            throw new NotImplementedException();
        }

    }
}