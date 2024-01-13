using GenFree.Interfaces.DB;
using GenFree.Helper;
using GenFree.Interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using GenFree.Model.Data;

namespace GenFree.Data
{
    public class CSourceLinkData : CRSDataC<ESourceLinkProp, (int, EEventArt, int)>, ISourceLinkData
    {

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
            eArt = db_Table.Fields[SourceLinkFields.Art.AsFld()].AsEnum<EEventArt>();
            iLinkType = db_Table.Fields[SourceLinkFields._1.AsFld()].AsInt();
            iPersNr = db_Table.Fields[SourceLinkFields._2.AsFld()].AsInt();
            iLfdNr = db_Table.Fields[SourceLinkFields.LfNr.AsFld()].AsInt();
            iQuNr = db_Table.Fields[SourceLinkFields._3.AsFld()].AsInt();
            sField3 = db_Table.Fields[SourceLinkFields._4.AsFld()].AsString();
            sAus = db_Table.Fields[SourceLinkFields.Aus.AsFld()].AsString();
            sOrig = db_Table.Fields[SourceLinkFields.Orig.AsFld()].AsString();
            sKom = db_Table.Fields[SourceLinkFields.Kom.AsFld()].AsString();
        }

        public override Type GetPropType(ESourceLinkProp prop) => prop switch
        {
            ESourceLinkProp.eArt => typeof(EEventArt),
            ESourceLinkProp.iLinkType => typeof(int),
            ESourceLinkProp.iPersNr => typeof(int),
            ESourceLinkProp.iQuNr => typeof(int),
            ESourceLinkProp.sField3 => typeof(string),
            ESourceLinkProp.iLfdNr => typeof(int),
            ESourceLinkProp.sAus => typeof(string),
            ESourceLinkProp.sOrig => typeof(string),
            ESourceLinkProp.sKom => typeof(string),
            _ => throw new NotImplementedException()
        };

        public override object GetPropValue(ESourceLinkProp prop) => prop switch
        {
            ESourceLinkProp.eArt => eArt,
            ESourceLinkProp.iLinkType => iLinkType,
            ESourceLinkProp.iPersNr => iPersNr,
            ESourceLinkProp.iQuNr => iQuNr,
            ESourceLinkProp.sField3 => sField3,
            ESourceLinkProp.iLfdNr => iLfdNr,
            ESourceLinkProp.sAus => sAus,
            ESourceLinkProp.sOrig => sOrig,
            ESourceLinkProp.sKom => sKom,
            _ => throw new NotImplementedException()
        };

        protected override IRecordset? Seek((int, EEventArt, int) iD)
        {
            _db_Table.Index = nameof(SourceLinkIndex.Tab22);
            _db_Table.Seek("=", iD.Item1, iD.Item2, iD.Item3);
            return _db_Table.NoMatch ? null : _db_Table;
        }

        public override void SetDBValue(IRecordset dB_Table, string[]? asProps)
        {
            asProps ??= _changedPropsList.Select(e => e.ToString()).ToArray();
            foreach (var prop in asProps)
            {
                switch (prop.AsEnum<ESourceLinkProp>())
                {
                    case ESourceLinkProp.eArt:
                        dB_Table.Fields[SourceLinkFields.Art.AsFld()].Value = eArt;
                        break;
                    case ESourceLinkProp.iLinkType:
                        dB_Table.Fields[SourceLinkFields._1.AsFld()].Value = iLinkType;
                        break;
                    case ESourceLinkProp.iPersNr:
                        dB_Table.Fields[SourceLinkFields._2.AsFld()].Value = iPersNr;
                        break;
                    case ESourceLinkProp.iQuNr:
                        dB_Table.Fields[SourceLinkFields._3.AsFld()].Value = iQuNr;
                        break;
                    case ESourceLinkProp.iLfdNr:
                        dB_Table.Fields[SourceLinkFields.LfNr.AsFld()].Value = iLfdNr;
                        break;
                    case ESourceLinkProp.sField3:
                        dB_Table.Fields[SourceLinkFields._4.AsFld()].Value = sField3;
                        break;
                    case ESourceLinkProp.sAus:
                        dB_Table.Fields[SourceLinkFields.Aus.AsFld()].Value = sAus;
                        break;
                    case ESourceLinkProp.sKom:
                        dB_Table.Fields[SourceLinkFields.Kom.AsFld()].Value = sKom;
                        break;
                    case ESourceLinkProp.sOrig:
                        dB_Table.Fields[SourceLinkFields.Orig.AsFld()].Value = sOrig;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public override void SetPropValue(ESourceLinkProp prop, object value)
        {
            if (EqualsProp(prop, value))
                return;
            AddChangedProp(prop);

            object _ = prop switch
            {
                ESourceLinkProp.eArt => eArt = (EEventArt)value,
                ESourceLinkProp.iLinkType => iLinkType = (int)value,
                ESourceLinkProp.iPersNr => iPersNr = (int)value,
                ESourceLinkProp.iQuNr => iQuNr = (int)value,
                ESourceLinkProp.iLfdNr => iLfdNr = (int)value,
                ESourceLinkProp.sField3 => sField3 = (string)value,
                ESourceLinkProp.sAus => sAus = (string)value,
                ESourceLinkProp.sKom => sKom = (string)value,
                ESourceLinkProp.sOrig => sOrig = (string)value,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
