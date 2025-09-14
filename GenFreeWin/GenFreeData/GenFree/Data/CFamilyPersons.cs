using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using GenFree.Sys;
using GenFree.Models.Data;
using BaseLib.Helper;
using GenFree.Interfaces.Data;

namespace GenFree.Data
{
    public class CFamilyPersons : CRSDataInt<EFamilyProp>, IFamilyPersons, IFamilyData
    {
        private CArrayProxy<int> _kind;
        private CArrayProxy<string> _kiatext;
        private static Func<int, string> _getText = DataModul.TextLese1;
        private static Func<IRecordset> _getTable = () => DataModul.DB_FamilyTable;
        private string? _sName;
        private string? _sPrefix;
        private string? _sSuffix;

        public int Mann { get; set; }
        public int Frau { get; set; }

        public IArrayProxy<int> Kind => _kind;
        public IArrayProxy<string> KiAText => _kiatext;
        public IList<(int nr, string aTxt)> Kinder { get; } = new List<(int nr, string aTxt)>();

        public DateTime dAnlDatum { get; internal set; }
        public DateTime dEditDat { get; internal set; }
        public int iName { get; internal set; }
        public string sName => _sName ??= _getText(iName);
        public int iPrae { get; internal set; }
        public string sPrefix => _sPrefix ??= _getText(iPrae);
        public int iSuf { get; internal set; }
        public string sSuffix => _sSuffix ??= _getText(iSuf);
        public int iEltern { get; internal set; }
        public string sPruefen { get; internal set; } = "";
        public string[] sBem { get; } = new string[4];
        public int iGgv { get; internal set; }
        public bool xAeB { get; internal set; }

        public Guid? gUid { get; internal set; }

        protected override Enum _keyIndex => FamilyIndex.Fam;

        public IList<IEventData> Events { get; } =[];

        public IList<ILinkData> Connects { get; } =[];

        public CFamilyPersons() : this(_getTable(),true) { }

        public CFamilyPersons(IRecordset dB_FamilyTable, bool xNoInit=false) : base(dB_FamilyTable,xNoInit)
        {
            sBem.Initialize();
            Mann = 0;
            Frau = 0;
            _kind = new((i) => Kinder[i.AsInt()].nr, (i, v) => Kinder[i.AsInt()] = (v, Kinder[i.AsInt()].aTxt));
            _kiatext = new((i) => Kinder[i.AsInt()].aTxt, (i, v) => Kinder[i.AsInt()] = (Kinder[i.AsInt()].nr, v));
        }

        public override void FillData(IRecordset dB_FamilyTable)
        {
            if (dB_FamilyTable?.EOF != false) return;
            ReadID(dB_FamilyTable);
            dAnlDatum = dB_FamilyTable.Fields[FamilyFields.AnlDatum].AsDate();
            dEditDat = dB_FamilyTable.Fields[FamilyFields.EditDat].AsDate();
            iName = dB_FamilyTable.Fields[FamilyFields.Name].AsInt();
            iPrae = dB_FamilyTable.Fields[FamilyFields.Prae].AsInt();
            iSuf = dB_FamilyTable.Fields[FamilyFields.Suf].AsInt();
            sPruefen = dB_FamilyTable.Fields[FamilyFields.Prüfen].AsString();
            sBem[1] = dB_FamilyTable.Fields[FamilyFields.Bem1].AsString();
            sBem[2] = dB_FamilyTable.Fields[FamilyFields.Bem2].AsString();
            sBem[3] = dB_FamilyTable.Fields[FamilyFields.Bem3].AsString();
            iEltern = dB_FamilyTable.Fields[FamilyFields.Eltern].AsInt();
            iGgv = dB_FamilyTable.Fields[FamilyFields.ggv].AsInt();
            xAeB = dB_FamilyTable.Fields[FamilyFields.Aeb].AsBool();
            gUid = dB_FamilyTable.Fields[FamilyFields.Fuid].AsGUID();
            _sName = null;
            _sPrefix = null;
            _sSuffix = null;
        }

        public void Clear()
        {
            Mann = 0;
            Frau = 0;
            Kinder.Clear();
        }

        public void CheckSetAnlDatum(IRecordset dB_FamilyTable)
        {
            if (default == dAnlDatum)
            {
                dB_FamilyTable.Edit();
                dB_FamilyTable.Fields[FamilyFields.AnlDatum].Value = (dAnlDatum = DateTime.Today).ToString("yyyyMMdd");
                dB_FamilyTable.Fields[FamilyFields.EditDat].Value = "0";
                dB_FamilyTable.Update();
                dEditDat = default;
            }
        }

        public override void SetDBValues(IRecordset dB_PersonTable, Enum[]? asProps)
        {
            asProps ??= _changedPropsList.Select((e) => (Enum)e).ToArray();
            foreach (var prop in asProps)
            {
                switch (prop)
                {
                    case EFamilyProp.dAnlDatum:
                        dB_PersonTable.Fields[FamilyFields.AnlDatum].Value = dAnlDatum.ToString("yyyyMMdd");
                        break;
                    case EFamilyProp.dEditDat:
                        dB_PersonTable.Fields[FamilyFields.EditDat].Value = dEditDat.ToString("yyyyMMdd");
                        break;
                    case EFamilyProp.sPruefen:
                        dB_PersonTable.Fields[FamilyFields.Prüfen].Value = sPruefen;
                        break;
                    case EFamilyProp.sBem:
                        dB_PersonTable.Fields[FamilyFields.Bem1].Value = sBem[1];
                        dB_PersonTable.Fields[FamilyFields.Bem2].Value = sBem[2];
                        dB_PersonTable.Fields[FamilyFields.Bem3].Value = sBem[3];
                        break;
                    case EFamilyProp.ID:
                        dB_PersonTable.Fields[FamilyFields.FamNr].Value = ID;
                        break;
                    case EFamilyProp.xAeB:
                        dB_PersonTable.Fields[FamilyFields.Aeb].Value = xAeB ? "J" : " ";
                        break;
                    case EFamilyProp.iName:
                        dB_PersonTable.Fields[FamilyFields.Name].Value = iName;
                        break;
                    case EFamilyProp.iGgv:
                        dB_PersonTable.Fields[FamilyFields.ggv].Value = iGgv;
                        break;
                    case EFamilyProp.gUID:
                        dB_PersonTable.Fields[FamilyFields.Fuid].Value = gUid ?? Guid.Empty;
                        break;
                    case EFamilyProp.iPrae:
                        dB_PersonTable.Fields[FamilyFields.Prae].Value = iPrae;
                        break;
                    case EFamilyProp.iSuf:
                        dB_PersonTable.Fields[FamilyFields.Suf].Value = iSuf;
                        break;
                    case EFamilyProp.iEltern:
                        dB_PersonTable.Fields[FamilyFields.Eltern].Value = iEltern;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public override Type GetPropType(EFamilyProp prop)
        {
            return prop switch
            {
                EFamilyProp.dAnlDatum => typeof(DateTime),
                EFamilyProp.dEditDat => typeof(DateTime),
                EFamilyProp.sPruefen => typeof(string),
                EFamilyProp.sBem => typeof(string[]),
                EFamilyProp.ID => typeof(int),
                EFamilyProp.xAeB => typeof(bool),
                EFamilyProp.iName => typeof(int),
                EFamilyProp.iGgv => typeof(int),
                EFamilyProp.gUID => typeof(Guid),
                EFamilyProp.iPrae => typeof(int),
                EFamilyProp.iSuf => typeof(int),
                EFamilyProp.iEltern => typeof(int),
                _ => throw new NotImplementedException(),
            };
        }

        public override object? GetPropValue(EFamilyProp prop)
        {
            return prop switch
            {
                EFamilyProp.dAnlDatum => dAnlDatum,
                EFamilyProp.dEditDat => dEditDat,
                EFamilyProp.sPruefen => sPruefen,
                EFamilyProp.sBem => sBem,
                EFamilyProp.ID => ID,
                EFamilyProp.xAeB => xAeB,
                EFamilyProp.iName => iName,
                EFamilyProp.iGgv => iGgv,
                EFamilyProp.gUID => gUid,
                EFamilyProp.iPrae => iPrae,
                EFamilyProp.iSuf => iSuf,
                EFamilyProp.iEltern => iEltern,
                _ => throw new NotImplementedException(),
            };
        }

        public override void SetPropValue(EFamilyProp prop, object value)
        {
            Type t;
            try
            {
                t = GetPropType(prop);
                if ((bool?)t.GetMethod("Equals", new[] { t })!.Invoke(GetPropValue(prop), new[] { value }) ?? false)
                    return;
            }
            catch { t = typeof(object); }
            _changedPropsList.Add(prop);
            object _ = prop switch
            {
                EFamilyProp.dAnlDatum => dAnlDatum = (DateTime)value,
                EFamilyProp.dEditDat => dEditDat = (DateTime)value,
                EFamilyProp.sPruefen => sPruefen = (string)value,
                EFamilyProp.sBem when value is ListItem<int> l => sBem[l.ItemData] = l.ItemString,
                EFamilyProp.sBem => ((string[])value).IntoString(sBem),
                EFamilyProp.ID => _ID = (int)value,
                EFamilyProp.xAeB => xAeB = (bool)value,
                EFamilyProp.iName => iName = (int)value,
                EFamilyProp.iGgv => iGgv = (int)value,
                EFamilyProp.gUID => gUid = (Guid)value,
                EFamilyProp.iPrae => iPrae = (int)value,
                EFamilyProp.iSuf => iSuf = (int)value,
                EFamilyProp.iEltern => iEltern = (int)value,
                _ => throw new NotImplementedException(),
            };
        }


        public static void SetGetText(Func<int, string> getTextFnc)
        {
            _getText = getTextFnc;
        }

        public static void SetTableGtr(Func<IRecordset> value)
        {
            _getTable = value;
        }

        public override void ReadID(IRecordset dB_FamilyTable)
        {
            _ID = dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
        }
    }
}