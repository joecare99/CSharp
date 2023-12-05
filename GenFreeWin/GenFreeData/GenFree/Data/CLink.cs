using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Helper;
using System;
using System.Collections.Generic;

namespace GenFree.Data;

public class CLink : CUsesRecordSet<(int iFamily, int iPerson, ELinkKennz eKennz)>, ILink
{
    private Func<IRecordset> _DB_LinkTable;

    protected override IRecordset _db_Table => _DB_LinkTable();

    protected override string _keyIndex => nameof(LinkIndex.FamPruef);

    public CLink(Func<IRecordset> dB_LinkTable)
    {
        _DB_LinkTable = dB_LinkTable;
    }

    public int AppendFamilyParent(int famInArb, int persInArb, ELinkKennz kennz, Func<int, bool, ELinkKennz, int> CheckPerson, bool xIgnoreSex = false)
    {
        IRecordset dB_LinkTable = _db_Table;
        dB_LinkTable.Index = nameof(LinkIndex.FamSu);
        dB_LinkTable.Seek("=", famInArb, kennz);
        if (!dB_LinkTable.NoMatch)
        {
            return 0; // Nothing to do
        }

        int iRes;
        if ((iRes = CheckPerson?.Invoke(persInArb, xIgnoreSex, kennz) ?? 0) < 0)
            return iRes;
        InternalAppend(famInArb, persInArb, kennz);

        return 1;

    }

    public bool Delete(int iFamNr, int iPersNr, ELinkKennz eKennz) => Delete((iFamNr, iPersNr, eKennz));

    public bool DeleteAllE(int iPersNr, ELinkKennz iKennz)
    {
        bool result = false;
        _db_Table.Index = nameof(LinkIndex.ElSu);
        _db_Table.Seek("=", iPersNr, iKennz);
        while (!_db_Table.NoMatch
            && !_db_Table.EOF
            && _db_Table.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt() == iPersNr
            && _db_Table.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>() == iKennz)
        {
            _db_Table.Delete();
            result = true;
        }
        return result;
    }
    public bool DeleteAllF(int iFamNr, ELinkKennz iKennz)
    {
        bool result = false;
        _db_Table.Index = nameof(LinkIndex.FamSu);
        _db_Table.Seek("=", iFamNr, iKennz);
        while (!_db_Table.NoMatch
            && !_db_Table.EOF
            && _db_Table.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt() == iFamNr
            && _db_Table.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>() == iKennz)
        {
            _db_Table.Delete();
            result = true;
        }

        return result;
    }

    public void DeleteFamWhere(int famInArb, Predicate<ILinkData> pWhere)
    {
        ForEachFam(famInArb, (link, dB_LinkTable) =>
        {
            if (pWhere(link))
            {
                dB_LinkTable.Delete();
            }
        });
    }

    private void ForEachFam(int famInArb, Action<ILinkData, IRecordset> action, int iMaxIter = 100)
    {
        var dB_LinkTable = _db_Table;
        dB_LinkTable.Index = nameof(LinkIndex.FamNr);
        dB_LinkTable.Seek("=", famInArb);
        var M1_Iter = 1;
        while (M1_Iter++ <= iMaxIter
             && !dB_LinkTable.EOF
             && !dB_LinkTable.NoMatch
             && !(dB_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt() != famInArb))
        {
            try { action(new CLinkData(dB_LinkTable), dB_LinkTable); } catch { };
            dB_LinkTable.MoveNext();
        }
    }

    public bool DeleteQ<T>(int iFamNr, int iPersNr, ELinkKennz iKennz, T okVal, Func<int, int, T> func) where T : struct
    {
        bool result;
        _db_Table.Index = _keyIndex;
        _db_Table.Seek("=", iFamNr, iPersNr, iKennz);
        if ((result = !_db_Table.NoMatch) && okVal!.Equals(func(iPersNr, iFamNr)))
            _db_Table.Delete();

        return result;
    }

    public void DeleteInvalidPerson()
    {
        _db_Table.Index = nameof(LinkIndex.Per);
        _db_Table.Seek("=", 0);
        while (!_db_Table.EOF
            && !_db_Table.NoMatch
            && _db_Table.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt() <= 0)
        {
            _db_Table.Delete();
            _db_Table.MoveNext();
        }
    }

    public bool SetVerknQ<T>(int iFamNr, int iPersNr, ELinkKennz iKennz, T okVal, Func<int, int, T> func) where T : struct
    {
        bool result;
        _db_Table.Index = _keyIndex;
        _db_Table.Seek("=", iFamNr, iPersNr, iKennz);
        if ((result = !_db_Table.NoMatch) && okVal.Equals(func(iPersNr, iFamNr)))
            _db_Table.Delete();
        else
        {
            InternalAppend(iFamNr, iPersNr, iKennz);
        }
        return result;
    }

    public bool SetEQ<T>(int iFamNr, int iPersNr, ELinkKennz iKennz, T okVal, Func<int, int, T> func) where T : struct
    {
        bool result;
        _db_Table.Index = nameof(LinkIndex.ElSu);
        _db_Table.Seek("=", iPersNr, iKennz);
        if ((result = !_db_Table.NoMatch) && okVal.Equals(func(iPersNr, iFamNr)))
            _db_Table.Delete();
        else
        {
            InternalAppend(iFamNr, iPersNr, iKennz);
        }
        return result;
    }

    public void Append(int iFamily, int iPerson, ELinkKennz iKennz)
    {
        _db_Table.Index = _keyIndex;
        _db_Table.Seek("=", iFamily, iPerson, iKennz);
        if (_db_Table.NoMatch)
        {
            InternalAppend(iFamily, iPerson, iKennz);
        }
    }


    public bool AppendE(int iFamNr, int iPerson, ELinkKennz eKennz)
    {
        bool result;
        _db_Table.Index = nameof(LinkIndex.ElSu);
        _db_Table.Seek("=", iPerson, eKennz);
        if (result = _db_Table.NoMatch)
        {
            InternalAppend(iFamNr, iPerson, eKennz);
        }
        return result;
    }
    private void InternalAppend(int iFamily, int iPerson, ELinkKennz iKennz)
    {
        _db_Table.AddNew();
        _db_Table.Fields[nameof(ILinkData.LinkFields.FamNr)].Value = iFamily;
        _db_Table.Fields[nameof(ILinkData.LinkFields.PerNr)].Value = iPerson;
        _db_Table.Fields[nameof(ILinkData.LinkFields.Kennz)].Value = iKennz;
        _db_Table.Update();
    }

    public bool Exist(int iFamily, int persInArb, ELinkKennz eKennz) => Exists((iFamily, persInArb, eKennz));
    public bool ExistE(int persInArb, ELinkKennz eKennz)
    {
        return SeekElSu(persInArb, eKennz, out _) != null;
    }
    public bool ExistF(int iFamily, ELinkKennz eKennz)
    {
        return SeekFaSu(iFamily, eKennz, out _) != null;
    }

    public bool ExistP(int iPersNr)
    {
        _db_Table.Index = nameof(LinkIndex.Per);
        _db_Table.Seek("=", iPersNr);
        return !_db_Table.NoMatch;
    }

    public bool ExistFam(int iFamily, ELinkKennz[] eLinkKennzs)
    {
        _db_Table.Index = nameof(LinkIndex.FamNr);
        _db_Table.Seek("=", iFamily);
        if (_db_Table.NoMatch)
            return false;
        var Link_iKennz = _db_Table.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>();
        foreach (var ekz in eLinkKennzs)
            if (Link_iKennz == ekz)
                return true;
        return false;
    }


    public void ReadFamily(int iFamily, IFamilyPersons Family, Action<ELinkKennz, int>? action = null)
    {
        Family_InitLinks(Family);
        ForEachFam(iFamily, (link, dB_LinkTable) =>
        {
            Family_SetLinkPerson(Family, action, link.iPersNr, link.eKennz);
        });
        Family.Kinder[0] = (Family.Kinder.Count - 1, "");
    }

    private static void Family_InitLinks(IFamilyPersons Family)
    {
        Family.Mann = 0;
        Family.Frau = 0;
        Family.Kinder.Clear();
        Family.Kinder.Add((0, ""));
    }

    private static void Family_SetLinkPerson(IFamilyPersons Family, Action<ELinkKennz, int>? action, int PeronNr, ELinkKennz value)
    {
        switch (value)
        {
            case ELinkKennz.lkFather:
                Family.Mann = PeronNr;
                break;
            case ELinkKennz.lkMother:
                Family.Frau = PeronNr;
                break;
            case ELinkKennz.lkChild:
                Family.Kinder.Add((PeronNr, ""));
                break;
            case ELinkKennz.lkAdoptedChild:
                Family.Kinder.Add((PeronNr, "A"));
                break;
            default:
                try { action?.Invoke(value, PeronNr); } catch { };
                break;
        }
    }

    public T DeleteChildren<T>(int iFamily, ELinkKennz iKennz, int persInArb, T mrOK, Func<int, T> func) where T : struct
    {
        IRecordset dB_LinkTable = _db_Table;
        dB_LinkTable.Index = _keyIndex;
        dB_LinkTable.Seek("=", iFamily, persInArb, iKennz);
        if (!dB_LinkTable.NoMatch)
        {
            var mr = func(dB_LinkTable.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt());
            if (!mr.Equals(mrOK))
                return mr;
            dB_LinkTable.Delete();
        }
        return mrOK;
    }

    public bool GetPersonFam(int persInArb, ELinkKennz eLKennz, out int iFamily)
    {
        var db_Table = SeekElSu(persInArb, eLKennz, out var xBreak);
        iFamily = xBreak ? 0 : db_Table!.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt();
        return !xBreak;
    }

    public IList<int> GetPersonFams(int persInArb, ELinkKennz eLKnz)
    {
        var _aiPers = new List<int>();
        foreach (var link in CLinkData.ReadPersLinkDataDB(persInArb, eLKnz))
        {
            _aiPers.Add(link.iFamNr);
        }
        return _aiPers;
    }

    public bool GetFamPerson(int iFamily, ELinkKennz eLKennz, out int Link_iPerNr)
    {
        var db_Table = SeekFaSu(iFamily, eLKennz, out var xBreak);
        Link_iPerNr = xBreak ? 0 : db_Table!.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt();
        return !xBreak;
    }

    public void DeleteFam(int iFamily, ELinkKennz iKennz)
    {
        SeekFaSu(iFamily, iKennz, out _)?.Delete();
    }
    public IRecordset? SeekFaSu(int iFamily, ELinkKennz eKennz, out bool xBreak)
    {
        IRecordset dB_LinkTable = _db_Table;
        dB_LinkTable.Index = nameof(LinkIndex.FamSu);
        dB_LinkTable.Seek("=", iFamily, eKennz);
        xBreak = dB_LinkTable.NoMatch;
        return xBreak ? null : dB_LinkTable;
    }

    public IRecordset? SeekElSu(int iPerson, ELinkKennz eKennz, out bool xBreak)
    {
        IRecordset dB_LinkTable = _db_Table;
        dB_LinkTable.Index = nameof(LinkIndex.ElSu);
        dB_LinkTable.Seek("=", iPerson, eKennz);
        xBreak = dB_LinkTable.NoMatch;
        return xBreak ? null : dB_LinkTable;
    }

    public override IRecordset? Seek((int iFamily, int iPerson, ELinkKennz eKennz) key, out bool xBreak)
    {
        _db_Table.Index = _keyIndex;
        _db_Table.Seek("=", key.iFamily, key.iPerson, key.eKennz);
        xBreak = _db_Table.NoMatch;
        return xBreak ? null : _db_Table;
    }

    protected override (int iFamily, int iPerson, ELinkKennz eKennz) GetID(IRecordset recordset)
    {
        return (_db_Table.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt(),
                           _db_Table.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt(),
                                          _db_Table.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>());

    }
}
