﻿using BaseLib.Helper;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Models.Data;
using System;
using System.Linq;

namespace GenFree.Data
{
    public class COFBData : CRSDataC<EOFBProps, (int, string, int)>, IOFBData
    {
        private string? _sText;
        private static Func<int, string> _getText = DataModul.TextLese1;
     //   private static Func<IRecordset> _getTable;

        public override (int, string, int) ID => (iPerNr,sKennz,iTextNr);

        public int iPerNr { get; private set; }

        public string sKennz { get; private set; } = "";

        public int iTextNr { get; private set; }

        public string sText => _sText ??= _getText(iTextNr);

        public static void SetGetText(Func<int, string> getTextFnc)
        {
            _getText = getTextFnc;
        }

        //public static void SetTableGtr(Func<IRecordset> value)
        //{
        //    _getTable = value;
        //}
        public COFBData(IRecordset db_Table, bool xNoInit=false) : base(db_Table,xNoInit)
        {
        }

        public override void ReadID(IRecordset dB_Table)
        {
            iPerNr = dB_Table.Fields[OFBFields.PerNr].AsInt();
            sKennz = dB_Table.Fields[OFBFields.Kennz].AsString();
            iTextNr = dB_Table.Fields[OFBFields.TextNr].AsInt();
        }

        public override void FillData(IRecordset dB_Table)
        {
            ReadID(dB_Table);
        }

        public override Type GetPropType(EOFBProps prop)
        {
            return prop switch
            {
                EOFBProps.iPerNr => typeof(int),
                EOFBProps.sKennz => typeof(string),
                EOFBProps.iTextNr => typeof(int),
                EOFBProps.sText => typeof(string),
                _ => throw new NotImplementedException(),
            };
        }

        public override object? GetPropValue(EOFBProps prop)
        {
            return prop switch
            {
                EOFBProps.iPerNr => iPerNr,
                EOFBProps.sKennz => sKennz,
                EOFBProps.iTextNr => iTextNr,
                EOFBProps.sText => sText,
                _ => throw new NotImplementedException(),
            };
        }

        public override void SetDBValues(IRecordset dB_Table, Enum[]? asProps)
        {
            asProps ??= _changedPropsList.Select(e=>(Enum)e).ToArray();
            {
                foreach (var sProp in asProps)
                {
                    switch (sProp.AsEnum<EOFBProps>())
                    {
                        case EOFBProps.iPerNr:
                            dB_Table.Fields[OFBFields.PerNr].Value = iPerNr;
                            break;
                        case EOFBProps.sKennz:
                            dB_Table.Fields[OFBFields.Kennz].Value = sKennz;
                            break;
                        case EOFBProps.iTextNr:
                            dB_Table.Fields[OFBFields.TextNr].Value = iTextNr;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        public override void SetPropValue(EOFBProps prop, object value)
        {
           if(EqualsProp(prop, value))
            {
                return;
            }
            AddChangedProp(prop);   
            switch (prop)
            {
                case EOFBProps.iPerNr:
                    iPerNr = (int)value;
                    break;
                case EOFBProps.sKennz:
                    sKennz = (string)value;
                    break;
                case EOFBProps.iTextNr:
                    iTextNr = (int)value;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        protected override IRecordset? Seek((int, string, int) iD)
        {
            _db_Table.Index = $"{OFBIndex.Indn}";
            _db_Table.Seek("=", iD.Item1,iD.Item2,iD.Item3);
            return _db_Table.NoMatch ? null : _db_Table;
        }

    }
}