﻿using GenFree.Model.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using BaseLib.Helper;

namespace GenFree.Data
{
    /// <summary>PLace Data-class<br/>A class for Place data</summary>
    /// <example>
    /// <code>_ = new CPlaceData(rs);</code></example>
    /// <seealso cref="GenFree.Interfaces.IPlaceData" />
    public class CPlaceData : CRSData<EPlaceProp, int>, IPlaceData
    {
        private List<EPlaceProp> _changedPropList = new();

        private static Func<int, string> _GetText;
        private static Func<IRecordset> __dB_PlaceTable;
        private int iOrt1;
        private int iOrtsteil1;
        private int iKreis1;
        private int iLand1;
        private int iStaat1;
        private string? sBem1;
        private string? _sOrt;
        private string? _sOrtsteil;
        private string? _sKreis;
        private string? _sLand;
        private string? _sStaat;
        private int _ID;

        public static void SetGetText(Func<int, string> getText)
        {
            _GetText = getText;
        }

        public CPlaceData(IRecordset dB_PlaceTable) : base(dB_PlaceTable) { }

        public override void FillData(IRecordset dB_PlaceTable)
        {
            _ID = dB_PlaceTable.Fields[nameof(PlaceFields.OrtNr)].AsInt();
            iOrt1 = dB_PlaceTable.Fields[nameof(PlaceFields.Ort)].AsInt();
            iOrtsteil1 = dB_PlaceTable.Fields[nameof(PlaceFields.Ortsteil)].AsInt();
            iKreis1 = dB_PlaceTable.Fields[nameof(PlaceFields.Kreis)].AsInt();
            iLand1 = dB_PlaceTable.Fields[nameof(PlaceFields.Land)].AsInt();
            iStaat1 = dB_PlaceTable.Fields[nameof(PlaceFields.Staat)].AsInt();
            sStaatk = dB_PlaceTable.Fields[nameof(PlaceFields.Staatk)].AsString();
            sPLZ = dB_PlaceTable.Fields[nameof(PlaceFields.PLZ)].AsString();
            sTerr = dB_PlaceTable.Fields[nameof(PlaceFields.Terr)].AsString();
            sLoc = dB_PlaceTable.Fields[nameof(PlaceFields.Loc)].AsString();
            sL = dB_PlaceTable.Fields[nameof(PlaceFields.L)].AsString();
            sB = dB_PlaceTable.Fields[nameof(PlaceFields.B)].AsString();
            sBem1 = dB_PlaceTable.Fields[nameof(PlaceFields.Bem)].AsString();
            sZusatz = dB_PlaceTable.Fields[nameof(PlaceFields.Zusatz)].AsString();
            sGOV = dB_PlaceTable.Fields[nameof(PlaceFields.GOV)].AsString();
            sPolName = dB_PlaceTable.Fields[nameof(PlaceFields.PolName)].AsString();
            ig = dB_PlaceTable.Fields[nameof(PlaceFields.g)].AsInt();
            _sKreis = _sLand = _sOrt = _sOrtsteil = _sStaat = null;
        }

        public IReadOnlyList<EPlaceProp> ChangedProps => _changedPropList;

        public override int ID => _ID;
        public int iOrt { get => iOrt1; set => SetPropValue(EPlaceProp.iOrt, value); }
        public string sOrt => _sOrt ??= _GetText(iOrt);
        public int iOrtsteil { get => iOrtsteil1; set => SetPropValue(EPlaceProp.iOrtsteil, value); }
        public string sOrtsteil => _sOrtsteil ??= _GetText(iOrtsteil);
        public int iKreis { get => iKreis1; set => SetPropValue(EPlaceProp.iKreis, value); }
        public string sKreis => _sKreis ??= _GetText(iKreis);
        public int iLand { get => iLand1; set => SetPropValue(EPlaceProp.iLand, value); }
        public string sLand => _sLand ??= _GetText(iLand);
        public int iStaat { get => iStaat1; set => SetPropValue(EPlaceProp.iStaat, value); }
        public string sStaat => _sStaat ??= _GetText(iStaat);
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

        protected override Enum _keyIndex => PlaceIndex.OrtNr;

        public void AddChangedProp(EPlaceProp prop)
        {
            if (!_changedPropList.Contains(prop))
                _changedPropList.Add(prop);
        }

        public void ClearChangedProps()
        {
            _changedPropList.Clear();
        }

        public override Type GetPropType(EPlaceProp prop)
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
                EPlaceProp.sOrt => typeof(string),
                EPlaceProp.sOrtsteil => typeof(string),
                EPlaceProp.sKreis => typeof(string),
                EPlaceProp.sLand => typeof(string),
                EPlaceProp.sStaat => typeof(string),
                _ => throw new NotImplementedException(),
            };
        }

        public override object GetPropValue(EPlaceProp prop)
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
                EPlaceProp.sOrt => sOrt,
                EPlaceProp.sOrtsteil => sOrtsteil,
                EPlaceProp.sKreis => sKreis,
                EPlaceProp.sLand => sLand,
                EPlaceProp.sStaat => sStaat,
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>Sets the property value.</summary>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        public override void SetPropValue(EPlaceProp prop, object value)
        {
            if (EqualsProp(prop, value)) return;

            AddChangedProp(prop);

            object _ = prop switch
            {
                EPlaceProp.ID => _ID = (int)value,
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
                _ => throw new NotImplementedException()
            };
        }

        public override void SetDBValue(IRecordset dB_FamilyTable, Enum[]? asProps)
        {
            asProps ??= _changedPropList.Select((e) => (Enum)e).ToArray();
            foreach (var prop in asProps)
            {
                _ = prop.AsEnum<EPlaceProp>() switch
                {
                    EPlaceProp.ID => dB_FamilyTable.Fields[nameof(PlaceFields.OrtNr)].Value = ID,
                    EPlaceProp.iOrt => dB_FamilyTable.Fields[nameof(PlaceFields.Ort)].Value = iOrt,
                    EPlaceProp.iOrtsteil => dB_FamilyTable.Fields[nameof(PlaceFields.Ortsteil)].Value = iOrtsteil,
                    EPlaceProp.iKreis => dB_FamilyTable.Fields[nameof(PlaceFields.Kreis)].Value = iKreis,
                    EPlaceProp.iLand => dB_FamilyTable.Fields[nameof(PlaceFields.Land)].Value = iLand,
                    EPlaceProp.iStaat => dB_FamilyTable.Fields[nameof(PlaceFields.Staat)].Value = iStaat,
                    EPlaceProp.sStaatk => dB_FamilyTable.Fields[nameof(PlaceFields.Staatk)].Value = sStaatk,
                    EPlaceProp.sPLZ => dB_FamilyTable.Fields[nameof(PlaceFields.PLZ)].Value = sPLZ,
                    EPlaceProp.sTerr => dB_FamilyTable.Fields[nameof(PlaceFields.Terr)].Value = sTerr,
                    EPlaceProp.sLoc => dB_FamilyTable.Fields[nameof(PlaceFields.Loc)].Value = sLoc,
                    EPlaceProp.sL => dB_FamilyTable.Fields[nameof(PlaceFields.L)].Value = sL,
                    EPlaceProp.sB => dB_FamilyTable.Fields[nameof(PlaceFields.B)].Value = sB,
                    EPlaceProp.sBem => dB_FamilyTable.Fields[nameof(PlaceFields.Bem)].Value = sBem,
                    EPlaceProp.sZusatz => dB_FamilyTable.Fields[nameof(PlaceFields.Zusatz)].Value = sZusatz,
                    EPlaceProp.sGOV => dB_FamilyTable.Fields[nameof(PlaceFields.GOV)].Value = sGOV,
                    EPlaceProp.sPolName => dB_FamilyTable.Fields[nameof(PlaceFields.PolName)].Value = sPolName,
                    EPlaceProp.ig => dB_FamilyTable.Fields[nameof(PlaceFields.g)].Value = ig,
                    _ => throw new NotImplementedException(),
                };
            }
        }

    }
}