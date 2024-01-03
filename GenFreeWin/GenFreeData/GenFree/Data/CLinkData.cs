using GenFree.Helper;
using System;
using System.Collections.Generic;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;

namespace GenFree.Data
{

    public class CLinkData : ILinkData
    {
        private static Func<IRecordset> _DB_LinkTable;
        private List<ELinkProp> _changedPropList;

        private static IRecordset LinkTable => _DB_LinkTable();

        public ELinkKennz eKennz { get; internal set; }
        public int iKennz => eKennz.AsInt();
        public int iFamNr { get; internal set; }
        public int iPersNr { get; internal set; }

        public (int iFamily, int iPerson, ELinkKennz eKennz) ID => (iFamNr, iPersNr, eKennz);

        public IReadOnlyList<ELinkProp> ChangedProps => _changedPropList;

        static CLinkData()
        {
            _DB_LinkTable = () => DataModul.DB_LinkTable;
        }

        public CLinkData()
        {
        }

        public CLinkData(ELinkKennz kennz, int famInArb, int PersNr)
        {
            eKennz = kennz;
            iFamNr = famInArb;
            iPersNr = PersNr;
        }

        public CLinkData(IRecordset dB_LinkTable)
        {
            FillLink(dB_LinkTable);
        }
        public static void SetLinkTblGetter(Func<IRecordset> dbLinkGetter) => _DB_LinkTable = dbLinkGetter;

        public void FillLink(IRecordset dB_LinkTable)
        {
            eKennz = dB_LinkTable.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>();
            iFamNr = dB_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt();
            iPersNr = dB_LinkTable.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt();
        }

        public void AppendDB()
        {
            var dB_LinkTable = LinkTable;
            dB_LinkTable.AddNew();
            dB_LinkTable.Fields[nameof(ILinkData.LinkFields.Kennz)].Value = eKennz;
            dB_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].Value = iFamNr;
            dB_LinkTable.Fields[nameof(ILinkData.LinkFields.PerNr)].Value = iPersNr;
            dB_LinkTable.Update();
        }

        public void Delete()
        {
            var dB_LinkTable = LinkTable;
            dB_LinkTable.Index = nameof(LinkIndex.FamPruef);
            dB_LinkTable.Seek("=", iFamNr, iPersNr, iKennz);
            if (!dB_LinkTable.NoMatch)
                dB_LinkTable.Delete();
        }

        public void SetPers(int p2)
        {
            var dB_LinkTable = LinkTable;
            if (dB_LinkTable.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>() != eKennz
            || dB_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt() != iFamNr
            || dB_LinkTable.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt() != iPersNr)
            {
                dB_LinkTable.Index = nameof(LinkIndex.FamPruef);
                dB_LinkTable.Seek("=", iFamNr, iPersNr, iKennz);
                if (dB_LinkTable.NoMatch) return;
            }
            dB_LinkTable.Edit();
            dB_LinkTable.Fields[nameof(ILinkData.LinkFields.PerNr)].Value = p2;
            dB_LinkTable.Update();
        }

        public void SetFam(int _iFamNr)
        {
            var dB_LinkTable = LinkTable;
            if (dB_LinkTable.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>() != eKennz
            || dB_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt() != iFamNr
            || dB_LinkTable.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt() != iPersNr)
            {
                dB_LinkTable.Index = nameof(LinkIndex.FamPruef);
                dB_LinkTable.Seek("=", iFamNr, iPersNr, iKennz);
                if (dB_LinkTable.NoMatch) return;
            }
            dB_LinkTable.Edit();
            dB_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].Value = _iFamNr;
            dB_LinkTable.Update();
        }

        public Type GetPropType(ELinkProp prop)
        {
            return prop switch
            {
                ELinkProp.eKennz => typeof(ELinkKennz),
                ELinkProp.iFamNr => typeof(int),
                ELinkProp.iPersNr => typeof(int),
                _ => throw new NotImplementedException(),
            };
        }

        public object GetPropValue(ELinkProp prop)
        {
            return prop switch
            {
                ELinkProp.eKennz => eKennz,
                ELinkProp.iFamNr => iFamNr,
                ELinkProp.iPersNr => iPersNr,
                _ => throw new NotImplementedException(),
            };
        }

        public T2 GetPropValue<T2>(ELinkProp prop)
        {
           return (T2)GetPropValue(prop);
        }

        public void SetPropValue(ELinkProp prop, object value)
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

        public void ClearChangedProps()
        {
            _changedPropList.Clear();
        }

        public void AddChangedProp(ELinkProp prop)
        {
            _changedPropList.Add(prop);
        }
    }
}