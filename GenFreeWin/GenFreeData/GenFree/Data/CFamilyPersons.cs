using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Helper;
using System;
using System.Collections.Generic;

namespace GenFree.Data
{
    public class CFamilyPersons : IFamilyPersons, IFamilyData
    {
        private CArrayProxy<int> _kind;
        private CArrayProxy<string> _kiatext;

        public int Mann { get; set; }
        public int Frau { get; set; }

        public IArrayProxy<int> Kind => _kind;
        public IArrayProxy<string> KiAText => _kiatext;
        public IList<(int nr, string aTxt)> Kinder { get; } = new List<(int nr, string aTxt)>();
        public int ID { get; private set; }
        public DateTime dAnlDatum { get; internal set; }
        public DateTime dEditDat { get; internal set; }
        public int iName { get; internal set; }
        public string sName => iName > 0 ? DataModul.TextLese1(iName) : "";
        public int iPrae { get; internal set; }
        public string sPrefix => iPrae > 0 ? DataModul.TextLese1(iPrae) : "";
        public int iSuf { get; internal set; }
        public string sSuffix => iSuf > 0 ? DataModul.TextLese1(iSuf) : "";
        public string sPruefen { get; internal set; }
        public string[] sBem { get; } = new string[4];
        public int iGgv { get; internal set; }
        public bool xAeB { get; internal set; }

        public Guid? gUID { get; internal set; }

        public CFamilyPersons()
        {
            Mann = 0;
            Frau = 0;
            _kind = new((i) => Kinder[i.AsInt()].nr, (i, v) => Kinder[i.AsInt()] = (v, Kinder[i.AsInt()].aTxt));
            _kiatext = new((i) => Kinder[i.AsInt()].aTxt, (i, v) => Kinder[i.AsInt()] = (Kinder[i.AsInt()].nr, v));
        }

        public CFamilyPersons(IRecordset dB_FamilyTable) : this()
        {
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
            iGgv = dB_FamilyTable.Fields[nameof(FamilyFields.ggv)].AsInt();
            xAeB = dB_FamilyTable.Fields[nameof(FamilyFields.Aeb)].AsBool();
            gUID = dB_FamilyTable.Fields[nameof(FamilyFields.Fuid)].AsGUID();
        }

        public void Clear()
        {
            Mann = 0;
            Frau = 0;
            var M1_Iter = 1;
            while (M1_Iter <= 99)
            {
                Kind[M1_Iter] = 0;
                M1_Iter++;
            }
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
            throw new NotImplementedException();
        }
    }
}