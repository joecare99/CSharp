using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenFree.Data
{
    public class CFamilyPersons : IFamilyPersons, IFamilyData
    {
        private CArrayProxy<int> _kind;
        private CArrayProxy<string> _kiatext;
        private static Func<int, string>? _getText;
        private List<EFamilyProp> _changedPropsList = new();
        private static Func<IRecordset> _getTable;
        private IRecordset _db_Table;
        private string? _sName;
        private string? _sPrefix;
        private string? _sSuffix;

        public int Mann { get; set; }
        public int Frau { get; set; }

        public IArrayProxy<int> Kind => _kind;
        public IArrayProxy<string> KiAText => _kiatext;
        public IList<(int nr, string aTxt)> Kinder { get; } = new List<(int nr, string aTxt)>();
        public int ID { get; private set; }
        public DateTime dAnlDatum { get; internal set; }
        public DateTime dEditDat { get; internal set; }
        public int iName { get; internal set; }
        public string sName => _sName ??= _getText(iName);
        public int iPrae { get; internal set; }
        public string sPrefix => _sPrefix ??= _getText(iPrae);
        public int iSuf { get; internal set; }
        public string sSuffix => _sSuffix ??= _getText(iSuf);
        public int iEltern { get; internal set; }
        public string sPruefen { get; internal set; }
        public string[] sBem { get; } = new string[4];
        public int iGgv { get; internal set; }
        public bool xAeB { get; internal set; }

        public Guid? gUID { get; internal set; }

        public IReadOnlyList<EFamilyProp> ChangedProps => _changedPropsList;

        public CFamilyPersons()
        {
            sBem.Initialize();
            Mann = 0;
            Frau = 0;
            _kind = new((i) => Kinder[i.AsInt()].nr, (i, v) => Kinder[i.AsInt()] = (v, Kinder[i.AsInt()].aTxt));
            _kiatext = new((i) => Kinder[i.AsInt()].aTxt, (i, v) => Kinder[i.AsInt()] = (Kinder[i.AsInt()].nr, v));
        }

        public CFamilyPersons(IRecordset dB_FamilyTable) : this()
        {
            _db_Table = dB_FamilyTable;
            FillData(dB_FamilyTable);
        }

        public void FillData(IRecordset dB_FamilyTable)
        {
            ID = dB_FamilyTable.Fields[nameof(FamilyFields.FamNr)].AsInt();
            dAnlDatum = dB_FamilyTable.Fields[nameof(FamilyFields.AnlDatum)].AsDate();
            dEditDat = dB_FamilyTable.Fields[nameof(FamilyFields.EditDat)].AsDate();
            iName = dB_FamilyTable.Fields[nameof(FamilyFields.Name)].AsInt();
            iPrae = dB_FamilyTable.Fields[nameof(FamilyFields.Prae)].AsInt();
            iSuf = dB_FamilyTable.Fields[nameof(FamilyFields.Suf)].AsInt();
            sPruefen = dB_FamilyTable.Fields[nameof(FamilyFields.Prüfen)].AsString();
            sBem[1] = dB_FamilyTable.Fields[nameof(FamilyFields.Bem1)].AsString();
            sBem[2] = dB_FamilyTable.Fields[nameof(FamilyFields.Bem2)].AsString();
            sBem[3] = dB_FamilyTable.Fields[nameof(FamilyFields.Bem3)].AsString();
            iEltern = dB_FamilyTable.Fields[nameof(FamilyFields.Eltern)].AsInt();
            iGgv = dB_FamilyTable.Fields[nameof(FamilyFields.ggv)].AsInt();
            xAeB = dB_FamilyTable.Fields[nameof(FamilyFields.Aeb)].AsBool();
            gUID = dB_FamilyTable.Fields[nameof(FamilyFields.Fuid)].AsGUID();
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
                dB_FamilyTable.Fields[nameof(FamilyFields.AnlDatum)].Value = (dAnlDatum = DateTime.Today).ToString("yyyyMMdd");
                dB_FamilyTable.Fields[nameof(FamilyFields.EditDat)].Value = "0";
                dB_FamilyTable.Update();
                dEditDat = default;
            }
        }

        public void SetDBValue(IRecordset dB_PersonTable, string[]? asProps)
        {
            asProps ??= _changedPropsList.Select((e) => e.ToString()).ToArray();
            foreach (var prop in asProps)
            {
                switch (prop.AsEnum<EFamilyProp>())
                {
                    case EFamilyProp.dAnlDatum:
                        dB_PersonTable.Fields[nameof(FamilyFields.AnlDatum)].Value = dAnlDatum.ToString("yyyyMMdd");
                        break;
                    case EFamilyProp.dEditDat:
                        dB_PersonTable.Fields[nameof(FamilyFields.EditDat)].Value = dEditDat.ToString("yyyyMMdd");
                        break;
                    case EFamilyProp.sPruefen:
                        dB_PersonTable.Fields[nameof(FamilyFields.Prüfen)].Value = sPruefen;
                        break;
                    case EFamilyProp.sBem:
                        dB_PersonTable.Fields[nameof(FamilyFields.Bem1)].Value = sBem[1];
                        dB_PersonTable.Fields[nameof(FamilyFields.Bem2)].Value = sBem[2];
                        dB_PersonTable.Fields[nameof(FamilyFields.Bem3)].Value = sBem[3];
                        break;
                    case EFamilyProp.ID:
                        dB_PersonTable.Fields[nameof(FamilyFields.FamNr)].Value = ID;
                        break;
                    case EFamilyProp.xAeB:
                        dB_PersonTable.Fields[nameof(FamilyFields.Aeb)].Value = xAeB ? "J" : " ";
                        break;
                    case EFamilyProp.iName:
                        dB_PersonTable.Fields[nameof(FamilyFields.Name)].Value = iName;
                        break;
                    case EFamilyProp.iGgv:
                        dB_PersonTable.Fields[nameof(FamilyFields.ggv)].Value = iGgv;
                        break;
                    case EFamilyProp.gUID:
                        dB_PersonTable.Fields[nameof(FamilyFields.Fuid)].Value = gUID;
                        break;
                    case EFamilyProp.iPrae:
                        dB_PersonTable.Fields[nameof(FamilyFields.Prae)].Value = iPrae;
                        break;
                    case EFamilyProp.iSuf:
                        dB_PersonTable.Fields[nameof(FamilyFields.Suf)].Value = iSuf;
                        break;
                    case EFamilyProp.iEltern:
                        dB_PersonTable.Fields[nameof(FamilyFields.Eltern)].Value = iEltern;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Delete()
        {
            var dB_Table = _db_Table;
            dB_Table.Index = nameof(FamilyIndex.Fam);
            dB_Table.Seek("=", ID);
            if (!dB_Table.NoMatch)
                dB_Table.Delete();
        }

        public Type GetPropType(EFamilyProp prop)
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

        public object? GetPropValue(EFamilyProp prop)
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
                EFamilyProp.gUID => gUID,
                EFamilyProp.iPrae => iPrae,
                EFamilyProp.iSuf => iSuf,
                EFamilyProp.iEltern => iEltern,
                _ => throw new NotImplementedException(),
            };
        }

        public T2 GetPropValue<T2>(EFamilyProp prop)
        {
            return (T2)GetPropValue(prop);
        }

        public void SetPropValue(EFamilyProp prop, object value)
        {
            Type t;
            try
            {
                t = GetPropType(prop);
                if ((bool)t.GetMethod("Equals", new[] { t })!.Invoke(GetPropValue(prop), new[] { value }))
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
                EFamilyProp.ID => ID = (int)value,
                EFamilyProp.xAeB => xAeB = (bool)value,
                EFamilyProp.iName => iName = (int)value,
                EFamilyProp.iGgv => iGgv = (int)value,
                EFamilyProp.gUID => gUID = (Guid)value,
                EFamilyProp.iPrae => iPrae = (int)value,
                EFamilyProp.iSuf => iSuf = (int)value,
                EFamilyProp.iEltern => iEltern = (int)value,
                _ => throw new NotImplementedException(),
            };
        }

        public void ClearChangedProps()
        {
            _changedPropsList.Clear();
        }

        public void AddChangedProp(EFamilyProp prop)
        {
            if (!_changedPropsList.Contains(prop))
                _changedPropsList.Add(prop);
        }

        public static void SetGetText(Func<int, string> getTextFnc)
        {
            _getText = getTextFnc;
        }

        public static void SetTableGtr(Func<IRecordset> value)
        {
            _getTable = value;
        }
    }
}