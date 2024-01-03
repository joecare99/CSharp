using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenFree.Data
{
    public class CPlaceData : IPlaceData
    {
        private List<EPlaceProp> _changedPropList = new();
        private IRecordset? _dB_PlaceTable;

        private static Func<int, string> _GetText = DataModul.TextLese1;
        private static Func<IRecordset> __dB_PlaceTable;
        private int iOrt1;
        private int iOrtsteil1;
        private int iKreis1;
        private int iLand1;
        private int iStaat1;
        private string sBem1;

        public static void SetTable(Func<IRecordset> dB_PlaceTable)
        {
            __dB_PlaceTable = dB_PlaceTable;
        }

        public static void SetGetText(Func<int, string> getText)
        {
            _GetText = getText;
        }

        public CPlaceData(IRecordset dB_PlaceTable)
        {
            _dB_PlaceTable = dB_PlaceTable;
            FillData(_dB_PlaceTable);
        }

        private void FillData(IRecordset dB_PlaceTable)
        {
            ID = dB_PlaceTable.Fields[nameof(PlaceFields.OrtNr)].AsInt();
            iOrt = dB_PlaceTable.Fields[nameof(PlaceFields.Ort)].AsInt();
            iOrtsteil = dB_PlaceTable.Fields[nameof(PlaceFields.Ortsteil)].AsInt();
            iKreis = dB_PlaceTable.Fields[nameof(PlaceFields.Kreis)].AsInt();
            iLand = dB_PlaceTable.Fields[nameof(PlaceFields.Land)].AsInt();
            iStaat = dB_PlaceTable.Fields[nameof(PlaceFields.Staat)].AsInt();
            sStaatk = dB_PlaceTable.Fields[nameof(PlaceFields.Staatk)].AsString();
            sPLZ = dB_PlaceTable.Fields[nameof(PlaceFields.PLZ)].AsString();
            sTerr = dB_PlaceTable.Fields[nameof(PlaceFields.Terr)].AsString();
            sLoc = dB_PlaceTable.Fields[nameof(PlaceFields.Loc)].AsString();
            sL = dB_PlaceTable.Fields[nameof(PlaceFields.L)].AsString();
            sB = dB_PlaceTable.Fields[nameof(PlaceFields.B)].AsString();
            sBem = dB_PlaceTable.Fields[nameof(PlaceFields.Bem)].AsString();
            sZusatz = dB_PlaceTable.Fields[nameof(PlaceFields.Zusatz)].AsString();
            sGOV = dB_PlaceTable.Fields[nameof(PlaceFields.GOV)].AsString();
            sPolName = dB_PlaceTable.Fields[nameof(PlaceFields.PolName)].AsString();
            ig = dB_PlaceTable.Fields[nameof(PlaceFields.g)].AsInt();
        }

        public IReadOnlyList<EPlaceProp> ChangedProps => _changedPropList;

        public int ID { get; private set; }
        public int iOrt { get => iOrt1; set => SetPropValue(EPlaceProp.iOrt, value); }
        public string sOrt => _GetText(iOrt);
        public int iOrtsteil { get => iOrtsteil1; set => SetPropValue(EPlaceProp.iOrtsteil, value); }
        public string sOrtsteil => _GetText(iOrtsteil);
        public int iKreis { get => iKreis1; set => SetPropValue(EPlaceProp.iKreis, value); }
        public string sKreis => _GetText(iKreis);
        public int iLand { get => iLand1; set => SetPropValue(EPlaceProp.iLand, value); }
        public string sLand => _GetText(iLand);
        public int iStaat { get => iStaat1; set => SetPropValue(EPlaceProp.iStaat, value); }
        public string sStaat => _GetText(iStaat);
        public string sStaatk { get; private set; }
        public string sPLZ { get; private set; }
        public string sTerr { get; private set; }
        public string sLoc { get; private set; }
        public string sL { get; private set; }
        public string sB { get; private set; }
        public string sBem { get => sBem1; set => SetPropValue(EPlaceProp.sBem, value); }
        public string sZusatz { get; private set; }
        public string sGOV { get; private set; }
        public string sPolName { get; private set; }
        public int ig { get; private set; }

        public void AddChangedProp(EPlaceProp prop)
        {
            if (!_changedPropList.Contains(prop))
                _changedPropList.Add(prop);
        }

        public void ClearChangedProps()
        {
            _changedPropList.Clear();
        }

        public Type GetPropType(EPlaceProp prop)
        {
            return prop switch
            {
                EPlaceProp.ID => typeof(int),
                EPlaceProp.iOrt => typeof(int),
                EPlaceProp.iOrtsteil => typeof(int),
                EPlaceProp.iKreis => typeof(int),
                EPlaceProp.iLand => typeof(int),
                EPlaceProp.iStaat => typeof(int),
                EPlaceProp.sStaatk => typeof(string),
                EPlaceProp.sPLZ => typeof(string),
                EPlaceProp.sTerr => typeof(string),
                EPlaceProp.sLoc => typeof(string),
                EPlaceProp.sL => typeof(string),
                EPlaceProp.sB => typeof(string),
                EPlaceProp.sBem => typeof(string),
                EPlaceProp.sZusatz => typeof(string),
                EPlaceProp.sGOV => typeof(string),
                EPlaceProp.sPolName => typeof(string),
                EPlaceProp.ig => typeof(int),
                _ => throw new NotImplementedException(),
            };
        }

        public object GetPropValue(EPlaceProp prop)
        {
            return prop switch
            {
                EPlaceProp.ID => ID,
                EPlaceProp.iOrt => iOrt,
                EPlaceProp.iOrtsteil => iOrtsteil,
                EPlaceProp.iKreis => iKreis,
                EPlaceProp.iLand => iLand,
                EPlaceProp.iStaat => iStaat,
                EPlaceProp.sStaatk => sStaatk,
                EPlaceProp.sPLZ => sPLZ,
                EPlaceProp.sTerr => sTerr,
                EPlaceProp.sLoc => sLoc,
                EPlaceProp.sL => sL,
                EPlaceProp.sB => sB,
                EPlaceProp.sBem => sBem,
                EPlaceProp.sZusatz => sZusatz,
                EPlaceProp.sGOV => sGOV,
                EPlaceProp.sPolName => sPolName,
                EPlaceProp.ig => ig,
                _ => throw new NotImplementedException(),
            };
        }

        public T2 GetPropValue<T2>(EPlaceProp prop)
        {
            return (T2)GetPropValue(prop);
        }

        public void SetPropValue(EPlaceProp prop, object value)
        {
            if (GetPropType(prop).GetMethod("Equals")?.Invoke(GetPropValue(prop), new[] { value }) as bool? ?? false)
                return;            
            _changedPropList.Add(prop);
            object _ = prop switch
            {
                EPlaceProp.ID => ID = (int)value,
                EPlaceProp.iOrt => iOrt1 = (int)value,
                EPlaceProp.iOrtsteil => iOrtsteil1 = (int)value,
                EPlaceProp.iKreis => iKreis1 = (int)value,
                EPlaceProp.iLand => iLand1 = (int)value,
                EPlaceProp.iStaat => iStaat1 = (int)value,
                EPlaceProp.sStaatk => sStaatk = (string)value,
                EPlaceProp.sPLZ => sPLZ = (string)value,
                EPlaceProp.sTerr => sTerr = (string)value,
                EPlaceProp.sLoc => sLoc = (string)value,
                EPlaceProp.sL => sL = (string)value,
                EPlaceProp.sB => sB = (string)value,
                EPlaceProp.sBem => sBem1 = (string)value,
                EPlaceProp.sZusatz => sZusatz = (string)value,
                EPlaceProp.sGOV => sGOV = (string)value,
                EPlaceProp.sPolName => sPolName = (string)value,
                EPlaceProp.ig => ig = (int)value,
                _ => throw new NotImplementedException(),
            };
        }

        public void SetDBValue(IRecordset dB_FamilyTable, string[]? asProps)
        {
            asProps ??= _changedPropList.Select((e) => e.ToString()).ToArray();
            foreach (var prop in asProps)
            {
                switch (prop)
                {
                    case nameof(EPlaceProp.ID):
                        dB_FamilyTable.Fields[nameof(PlaceFields.OrtNr)].Value = ID;
                        break;
                    case nameof(EPlaceProp.iOrt):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Ort)].Value = iOrt;
                        break;
                    case nameof(EPlaceProp.iOrtsteil):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Ortsteil)].Value = iOrtsteil;
                        break;
                    case nameof(EPlaceProp.iKreis):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Kreis)].Value = iKreis;
                        break;
                    case nameof(EPlaceProp.iLand):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Land)].Value = iLand;
                        break;
                    case nameof(EPlaceProp.iStaat):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Staat)].Value = iStaat;
                        break;
                    case nameof(EPlaceProp.sStaatk):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Staatk)].Value = sStaatk;
                        break;
                    case nameof(EPlaceProp.sPLZ):
                        dB_FamilyTable.Fields[nameof(PlaceFields.PLZ)].Value = sPLZ;
                        break;
                    case nameof(EPlaceProp.sTerr):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Terr)].Value = sTerr;
                        break;
                    case nameof(EPlaceProp.sLoc):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Loc)].Value = sLoc;
                        break;
                    case nameof(EPlaceProp.sL):
                        dB_FamilyTable.Fields[nameof(PlaceFields.L)].Value = sL;
                        break;
                    case nameof(EPlaceProp.sB):
                        dB_FamilyTable.Fields[nameof(PlaceFields.B)].Value = sB;
                        break;
                    case nameof(EPlaceProp.sBem):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Bem)].Value = sBem;
                        break;
                    case nameof(EPlaceProp.sZusatz):
                        dB_FamilyTable.Fields[nameof(PlaceFields.Zusatz)].Value = sZusatz;
                        break;
                    case nameof(EPlaceProp.sGOV):
                        dB_FamilyTable.Fields[nameof(PlaceFields.GOV)].Value = sGOV;
                        break;
                    case nameof(EPlaceProp.sPolName):
                        dB_FamilyTable.Fields[nameof(PlaceFields.PolName)].Value = sPolName;
                        break;
                    case nameof(EPlaceProp.ig):
                        dB_FamilyTable.Fields[nameof(PlaceFields.g)].Value = ig;
                        break;
                }
            }
        }
    }
}