using GenFree.Interfaces.DB;
using GenFree.Helper;
using System;
using System.Linq;
using GenFree.Models.Data;
using BaseLib.Helper;
using GenFree.Interfaces.Data;

namespace GenFree.Data
{
    public class CSourceLinkData : CRSDataC<ESourceLinkProp, (short, int, EEventArt, short)>, ISourceLinkData
    {

        public CSourceLinkData(IRecordset recordset, bool xNoInit = false) : base(recordset, xNoInit)
        {
        }

        public short iLinkType { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); }
        public int iPerFamNr { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); }
        public EEventArt eArt { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); }
        public short iLfdNr { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); }
        public string sComment { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); } = "";
        public string sOriginalText { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); } = "";
        public string sPage { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); } = "";
        public string sEntry { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); } = "";
        public int iQuNr { get; set => SetPropValue(ref field, ESourceLinkProp.iLinkType, value); }

        public override (short, int, EEventArt, short) ID =>
            (iLinkType, iPerFamNr, eArt, (short)iLfdNr);

        public override void FillData(IRecordset db_Table)
        {
            ReadID(db_Table);
            iQuNr = db_Table.Fields[SourceLinkFields._3].AsInt();
            sEntry = db_Table.Fields[SourceLinkFields._4].AsString();
            sPage = db_Table.Fields[SourceLinkFields.Aus].AsString();
            sOriginalText = db_Table.Fields[SourceLinkFields.Orig].AsString();
            sComment = db_Table.Fields[SourceLinkFields.Kom].AsString();
        }

        public override Type GetPropType(ESourceLinkProp prop) => prop switch
        {
            ESourceLinkProp.eArt => typeof(EEventArt),
            ESourceLinkProp.iLinkType => typeof(short),
            ESourceLinkProp.iPerFamNr => typeof(int),
            ESourceLinkProp.iQuNr => typeof(int),
            ESourceLinkProp.sEntry => typeof(string),
            ESourceLinkProp.iLfdNr => typeof(short),
            ESourceLinkProp.sPage => typeof(string),
            ESourceLinkProp.sOriginalText => typeof(string),
            ESourceLinkProp.sComment => typeof(string),
            _ => throw new NotImplementedException()
        };

        public override object GetPropValue(ESourceLinkProp prop) => prop switch
        {
            ESourceLinkProp.eArt => eArt,
            ESourceLinkProp.iLinkType => iLinkType,
            ESourceLinkProp.iPerFamNr => iPerFamNr,
            ESourceLinkProp.iQuNr => iQuNr,
            ESourceLinkProp.sEntry => sEntry,
            ESourceLinkProp.iLfdNr => iLfdNr,
            ESourceLinkProp.sPage => sPage,
            ESourceLinkProp.sOriginalText => sOriginalText,
            ESourceLinkProp.sComment => sComment,
            _ => throw new NotImplementedException()
        };

        protected override IRecordset? Seek((short, int, EEventArt, short) iD)
        {
            _db_Table.Index = nameof(SourceLinkIndex.Tab22);
            _db_Table.Seek("=", iD.Item1, (int)iD.Item3, iD.Item2, iD.Item4);
            return _db_Table.NoMatch ? null : _db_Table;
        }

        public override void SetDBValues(IRecordset dB_Table, Enum[]? asProps)
        {
            asProps ??= _changedPropsList.Select(e => (Enum)e).ToArray();
            foreach (var prop in asProps)
            {
                switch (prop)
                {
                    case ESourceLinkProp.iLinkType:
                        dB_Table.Fields[SourceLinkFields._1].Value = iLinkType;
                        break;
                    case ESourceLinkProp.iPerFamNr:
                        dB_Table.Fields[SourceLinkFields._2].Value = iPerFamNr;
                        break;
                    case ESourceLinkProp.iQuNr:
                        dB_Table.Fields[SourceLinkFields._3].Value = iQuNr;
                        break;
                    case ESourceLinkProp.sEntry:
                        dB_Table.Fields[SourceLinkFields._4].Value = sEntry;
                        break;
                    case ESourceLinkProp.sPage:
                        dB_Table.Fields[SourceLinkFields.Aus].Value = sPage;
                        break;
                    case ESourceLinkProp.sOriginalText:
                        dB_Table.Fields[SourceLinkFields.Orig].Value = sOriginalText;
                        break;
                    case ESourceLinkProp.sComment:
                        dB_Table.Fields[SourceLinkFields.Kom].Value = sComment;
                        break;
                    case ESourceLinkProp.eArt:
                        dB_Table.Fields[SourceLinkFields.Art].Value = eArt;
                        break;
                    case ESourceLinkProp.iLfdNr:
                        dB_Table.Fields[SourceLinkFields.LfNr].Value = iLfdNr;
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
                ESourceLinkProp.iLinkType => iLinkType = (short)value,
                ESourceLinkProp.iPerFamNr => iPerFamNr = (int)value,
                ESourceLinkProp.iQuNr => iQuNr = (int)value,
                ESourceLinkProp.iLfdNr => iLfdNr = (short)value,
                ESourceLinkProp.sEntry => sEntry = (string)value,
                ESourceLinkProp.sPage => sPage = (string)value,
                ESourceLinkProp.sComment => sComment = (string)value,
                ESourceLinkProp.sOriginalText => sOriginalText = (string)value,
                _ => throw new NotImplementedException(),
            };
        }

        public override void ReadID(IRecordset db_Table)
        {
            eArt = db_Table.Fields[SourceLinkFields.Art].AsEnum<EEventArt>();
            iLinkType = (short)db_Table.Fields[SourceLinkFields._1].AsInt();
            iPerFamNr = db_Table.Fields[SourceLinkFields._2].AsInt();
            iLfdNr = (short)db_Table.Fields[SourceLinkFields.LfNr].AsInt();
        }
    }
}
