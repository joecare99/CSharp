using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Helper;
using GenFree.Interfaces;
using System.Collections.Generic;
using System;

namespace GenFree.Data
{
    public class CSourceLinkData : IHasID<(int, EEventArt, int)>, IHasPropEnum<ESourceLinkProp>
    {
        public CSourceLinkData()
        {
        }

        public CSourceLinkData(IRecordset recordset)
        {
            FillData(recordset);
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

        public (int, EEventArt, int) ID => throw new System.NotImplementedException();

        public IReadOnlyList<ESourceLinkProp> ChangedProps => throw new NotImplementedException();

        public void AddChangedProp(ESourceLinkProp prop)
        {
            throw new NotImplementedException();
        }

        public void ClearChangedProps()
        {
            throw new NotImplementedException();
        }

        public Type GetPropType(ESourceLinkProp prop)
        {
            throw new NotImplementedException();
        }

        public object GetPropValue(ESourceLinkProp prop)
        {
            throw new NotImplementedException();
        }

        public T2 GetPropValue<T2>(ESourceLinkProp prop)
        {
            throw new NotImplementedException();
        }

        public void SetPropValue(ESourceLinkProp prop, object value)
        {
            throw new NotImplementedException();
        }

        internal void FillData(IRecordset dB_TTable)
        {
            eArt = dB_TTable.Fields["Art"].AsEnum<EEventArt>();
            iLinkType = dB_TTable.Fields[0].AsInt();
            iPersNr = dB_TTable.Fields[1].AsInt();
            iLfdNr = dB_TTable.Fields["LfNr"].AsInt();
            iQuNr = dB_TTable.Fields["3"].AsInt();
            sField3 = dB_TTable.Fields[3].AsString();
            sAus = dB_TTable.Fields["Aus"].AsString();
            sOrig = dB_TTable.Fields["Orig"].AsString();
            sKom = dB_TTable.Fields["Kom"].AsString();            
        }
    }
}