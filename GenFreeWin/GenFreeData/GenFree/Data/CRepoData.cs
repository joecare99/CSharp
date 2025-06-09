using GenFree.Model.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using BaseLib.Helper;
using GenFree.Interfaces.Data;

namespace GenFree.Data
{
    /// <summary>PLace Data-class<br/>A class for Place data</summary>
    /// <example>
    /// <code>_ = new CPlaceData(rs);</code></example>
    /// <seealso cref="Interfaces.Data.IPlaceData" />
    public class CRepoData : CRSDataInt<ERepoProp>, IRepoData
    {
        private List<ERepoProp> _changedPropList = new();

        private string _sName = "";
        private string _sOrt = "";
        private string _sPLZ = "";
        private string _sStrasse = "";
        private string _sFon = "";
        private string _sMail = "";
        private string _sHttp = "";
        private string _sBem = "";
        private string _sSuchname = "";

        public override int ID => _ID;
        public string sName { get => _sName; set => SetPropValue(ERepoProp.sName, value); }
        public string sOrt { get => _sOrt; set => SetPropValue(ERepoProp.sOrt, value); }
        public string sPLZ { get => _sPLZ; set => SetPropValue(ERepoProp.sPLZ, value); }
        public string sStrasse { get => _sStrasse; set => SetPropValue(ERepoProp.sStrasse, value); }
        public string sFon { get => _sFon; set => SetPropValue(ERepoProp.sFon, value); }
        public string sMail { get => _sMail; set => SetPropValue(ERepoProp.sMail, value); }
        public string sHttp { get => _sHttp; set => SetPropValue(ERepoProp.sHttp, value); }
        public string sBem { get => _sBem; set => SetPropValue(ERepoProp.sBem, value); }
        public string sSuchname { get => _sSuchname; set => SetPropValue(ERepoProp.sSuchname, value); }

        protected override Enum _keyIndex => RepoIndex.Nr;

        public CRepoData(IRecordset dB_RepoTable, bool xNoInit=false) : base(dB_RepoTable,xNoInit) { }

        public override void ReadID(IRecordset dB_RepoTable)
        {
            _ID = dB_RepoTable.Fields[RepoFields.Nr].AsInt();
        }

        public override void FillData(IRecordset dB_RepoTable)
        {
            ReadID(dB_RepoTable);
            sOrt = dB_RepoTable.Fields[RepoFields.Ort].AsString();
            sPLZ = dB_RepoTable.Fields[RepoFields.PLZ].AsString();
            sStrasse = dB_RepoTable.Fields[RepoFields.Strasse].AsString();
            sFon = dB_RepoTable.Fields[RepoFields.Fon].AsString();
            sMail = dB_RepoTable.Fields[RepoFields.Mail].AsString();
            sHttp = dB_RepoTable.Fields[RepoFields.Http].AsString();
            sBem = dB_RepoTable.Fields[RepoFields.Bem].AsString();
            sSuchname = dB_RepoTable.Fields[RepoFields.Suchname].AsString();
        }

        public override Type GetPropType(ERepoProp prop)
        {
            return prop switch
            {
                ERepoProp.ID => typeof(int),
                ERepoProp.sName => typeof(string),
                ERepoProp.sOrt => typeof(string),
                ERepoProp.sPLZ => typeof(string),
                ERepoProp.sStrasse => typeof(string),
                ERepoProp.sFon => typeof(string),
                ERepoProp.sMail => typeof(string),
                ERepoProp.sHttp => typeof(string),
                ERepoProp.sBem => typeof(string),
                ERepoProp.sSuchname => typeof(string),
                _ => throw new NotImplementedException(),
            };
        }

        public override object GetPropValue(ERepoProp prop)
        {
            return prop switch
            {
                ERepoProp.ID => ID,
                ERepoProp.sName => sName,
                ERepoProp.sOrt => sOrt,
                ERepoProp.sPLZ => sPLZ,
                ERepoProp.sStrasse => sStrasse,
                ERepoProp.sFon => sFon,
                ERepoProp.sMail => sMail,
                ERepoProp.sHttp => sHttp,
                ERepoProp.sBem => sBem,
                ERepoProp.sSuchname => sSuchname,
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>Sets the property value.</summary>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        public override void SetPropValue(ERepoProp prop, object value)
        {
            if (EqualsProp(prop, value)) return;

            AddChangedProp(prop);

            object _ = prop switch
            {
                ERepoProp.ID => _ID = (int)value,
                ERepoProp.sName => _sName = (string)value,
                ERepoProp.sOrt => _sOrt = (string)value,
                ERepoProp.sPLZ => _sPLZ = (string)value,
                ERepoProp.sStrasse => _sStrasse = (string)value,
                ERepoProp.sFon => _sFon = (string)value,
                ERepoProp.sMail => _sMail = (string)value,
                ERepoProp.sHttp => _sHttp = (string)value,
                ERepoProp.sBem => _sBem = (string)value,
                ERepoProp.sSuchname => _sSuchname = (string)value,
                _ => throw new NotImplementedException()
            };
        }

        public override void SetDBValues(IRecordset dB_RepoTable, Enum[]? asProps)
        {
            asProps ??= _changedPropList.Select((e) => (Enum)e).ToArray();
            foreach (var prop in asProps)
            {
                _ = prop.AsEnum<ERepoProp>() switch
                {
                    ERepoProp.ID => dB_RepoTable.Fields[RepoFields.Nr].Value = ID,
                    ERepoProp.sName => dB_RepoTable.Fields[RepoFields.Name].Value = sName,
                    ERepoProp.sOrt => dB_RepoTable.Fields[RepoFields.Ort].Value = sOrt,
                    ERepoProp.sPLZ => dB_RepoTable.Fields[RepoFields.PLZ].Value = sPLZ,
                    ERepoProp.sStrasse => dB_RepoTable.Fields[RepoFields.Strasse].Value = sStrasse,
                    ERepoProp.sFon => dB_RepoTable.Fields[RepoFields.Fon].Value = sFon,
                    ERepoProp.sMail => dB_RepoTable.Fields[RepoFields.Mail].Value = sMail,
                    ERepoProp.sHttp => dB_RepoTable.Fields[RepoFields.Http].Value = sHttp,
                    ERepoProp.sBem => dB_RepoTable.Fields[RepoFields.Bem].Value = sBem,
                    ERepoProp.sSuchname => dB_RepoTable.Fields[RepoFields.Suchname].Value = sSuchname,
                    _ => throw new NotImplementedException(),
                };
            }
        }

    }
}