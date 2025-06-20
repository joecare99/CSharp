using GenFree.Helper;
using System;
using System.Collections.Generic;
using GenFree.Interfaces.DB;
using System.Linq;
using BaseLib.Helper;
using GenFree.Interfaces.Data;
using GenFree.Model.Data;

namespace GenFree.Data
{

    public class CLinkData : CRSDataC<ELinkProp, (int iFamily, int iPerson, ELinkKennz eKennz)>, ILinkData
    {
        private static Func<IRecordset>? _DB_LinkTable;

        public ELinkKennz eKennz { get; internal set; }
        public int iKennz => eKennz.AsInt();
        public int iFamNr { get; internal set; }
        public int iPersNr { get; internal set; }

        public override (int iFamily, int iPerson, ELinkKennz eKennz) ID => (iFamNr, iPersNr, eKennz);


        static CLinkData()
        {
            Reset();
        }

        public static void Reset()
        {
            _DB_LinkTable = () => DataModul.DB_LinkTable;
        }

        public CLinkData() : base(_DB_LinkTable?.Invoke()!, true)
        {
        }

        public CLinkData(ELinkKennz kennz, int famInArb, int PersNr) : this()
        {
            eKennz = kennz;
            iFamNr = famInArb;
            iPersNr = PersNr;
        }

        public CLinkData(IRecordset dB_LinkTable, bool xNoInit = false) : base(dB_LinkTable, xNoInit)
        {
        }

        public static void SetTableGtr(Func<IRecordset> fTblGtr) => _DB_LinkTable = fTblGtr;

        public override void FillData(IRecordset dB_LinkTable)
        {
            if (dB_LinkTable == null) return;
            ReadID(dB_LinkTable);
        }

        public void AppendDB()
        {
            var dB_LinkTable = _db_Table;
            dB_LinkTable.AddNew();
            dB_LinkTable.Fields[ILinkData.LinkFields.Kennz].Value = eKennz;
            dB_LinkTable.Fields[ILinkData.LinkFields.FamNr].Value = iFamNr;
            dB_LinkTable.Fields[ILinkData.LinkFields.PerNr].Value = iPersNr;
            dB_LinkTable.Update();
        }

        protected override IRecordset? Seek((int iFamily, int iPerson, ELinkKennz eKennz) iD)
        {
            var dB_LinkTable = _db_Table;
            dB_LinkTable.Index = nameof(LinkIndex.FamPruef);
            dB_LinkTable.Seek("=", ID.iFamily, ID.iPerson, ID.eKennz);
            return dB_LinkTable.NoMatch ? null : dB_LinkTable;
        }

        private bool CkeckRecordset(IRecordset dB_LinkTable, (int iFamily, int iPerson, ELinkKennz eKennz) ID)
        {
            var flag = true;
            if (dB_LinkTable.Fields[ILinkData.LinkFields.Kennz].AsEnum<ELinkKennz>() != ID.eKennz
            || dB_LinkTable.Fields[ILinkData.LinkFields.FamNr].AsInt() != ID.iFamily
            || dB_LinkTable.Fields[ILinkData.LinkFields.PerNr].AsInt() != ID.iPerson)
            {
                dB_LinkTable.Index = nameof(LinkIndex.FamPruef);
                dB_LinkTable.Seek("=", iFamNr, iPersNr, iKennz);
                flag = !dB_LinkTable.NoMatch;
            }
            return flag;
        }

        public void SetPers(int p2)
        {
            var dB_LinkTable = _db_Table;
            if (!CkeckRecordset(dB_LinkTable, ID)) return;
            dB_LinkTable.Edit();
            dB_LinkTable.Fields[ILinkData.LinkFields.PerNr].Value = p2;
            dB_LinkTable.Update();
            iPersNr = p2;
        }

        public void SetFam(int _iFamNr)
        {
            var dB_LinkTable = _db_Table;
            if (!CkeckRecordset(dB_LinkTable, ID)) return;
            dB_LinkTable.Edit();
            dB_LinkTable.Fields[ILinkData.LinkFields.FamNr].Value = _iFamNr;
            dB_LinkTable.Update();
            iFamNr = _iFamNr;
        }

        public override Type GetPropType(ELinkProp prop)
        {
            return prop switch
            {
                ELinkProp.eKennz => typeof(ELinkKennz),
                ELinkProp.iFamNr => typeof(int),
                ELinkProp.iPersNr => typeof(int),
                _ => throw new NotImplementedException(),
            };
        }

        public override object GetPropValue(ELinkProp prop)
        {
            return prop switch
            {
                ELinkProp.eKennz => eKennz,
                ELinkProp.iFamNr => iFamNr,
                ELinkProp.iPersNr => iPersNr,
                _ => throw new NotImplementedException(),
            };
        }

        public override void SetPropValue(ELinkProp prop, object value)
        {
            switch (prop)
            {
                case ELinkProp.eKennz:
                    if (eKennz == (ELinkKennz)value) return;
                    AddChangedProp(prop);
                    eKennz = (ELinkKennz)value;
                    break;
                case ELinkProp.iFamNr:
                    if (iFamNr == (int)value) return;
                    AddChangedProp(prop);
                    iFamNr = (int)value;
                    break;
                case ELinkProp.iPersNr:
                    if (iPersNr == (int)value) return;
                    AddChangedProp(prop);
                    iPersNr = (int)value;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


        public override void SetDBValues(IRecordset dB_Table, Enum[]? asProps)
        {
            asProps ??= _changedPropsList.Select((e) => (Enum)e).ToArray();
            foreach (var prop in asProps)
            {
                switch (prop)
                {
                    case ELinkProp.eKennz:
                        dB_Table.Fields[ILinkData.LinkFields.Kennz].Value = eKennz;
                        break;
                    case ELinkProp.iFamNr:
                        dB_Table.Fields[ILinkData.LinkFields.FamNr].Value = iFamNr;
                        break;
                    case ELinkProp.iPersNr:
                        dB_Table.Fields[ILinkData.LinkFields.PerNr].Value = iPersNr;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

        }

        public override void ReadID(IRecordset dB_LinkTable)
        {
            eKennz = dB_LinkTable.Fields[ILinkData.LinkFields.Kennz].AsEnum<ELinkKennz>();
            iFamNr = dB_LinkTable.Fields[ILinkData.LinkFields.FamNr].AsInt();
            iPersNr = dB_LinkTable.Fields[ILinkData.LinkFields.PerNr].AsInt();
        }

        
    }
}