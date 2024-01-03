using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using GenFree.Model;

namespace GenFree.Data;

public class CLink : CUsesIndexedRSet<(int iFamily, int iPerson, ELinkKennz eKennz),LinkIndex,ILinkData.LinkFields,ILinkData>, ILink
{
    private Func<IRecordset> _DB_LinkTable;

    protected override IRecordset _db_Table => _DB_LinkTable();

    protected override string _keyIndex => nameof(LinkIndex.FamPruef);

    public CLink(Func<IRecordset> dB_LinkTable)
    {
        _DB_LinkTable = dB_LinkTable;
    }

    public int AppendFamilyParent(int famInArb, int persInArb, ELinkKennz kennz, Func<int, bool, ELinkKennz, int>? CheckPerson, bool xIgnoreSex = false)
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

    private bool ForEachFam(int famInArb, Action<ILinkData, IRecordset> action, int iMaxIter = 100)
    {
        var dB_LinkTable = _db_Table;
        dB_LinkTable.Index = nameof(LinkIndex.FamNr);
        dB_LinkTable.Seek("=", famInArb);
        var M1_Iter = 1;
        bool xResult = !dB_LinkTable.NoMatch;
        while (M1_Iter++ <= iMaxIter
             && !dB_LinkTable.EOF
             && !dB_LinkTable.NoMatch
             && !(dB_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt() != famInArb))
        {
            try { action(new CLinkData(dB_LinkTable), dB_LinkTable); } catch { };
            dB_LinkTable.MoveNext();
        }
        return xResult;
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

    public bool ExistP(int iPersNr)=>Exists(LinkIndex.Per, iPersNr);

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


    public bool ReadFamily(int iFamily, IFamilyPersons Family, Action<ELinkKennz, int>? action = null)
    {
        Family_InitLinks(Family);
        var xResult = ForEachFam(iFamily, (link, dB_LinkTable) =>
        {
            Family_SetLinkPerson(Family, action, link.iPersNr, link.eKennz);
        });
        Family.Kinder[0] = (Family.Kinder.Count - 1, "");
        return xResult;
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
        foreach (var link in ReadAllPers(persInArb, eLKnz))
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

    /// <summary>
    /// Reads the link data from database. Using the index FamSu
    /// </summary>
    /// <param name="iFamNr">The i fam nr.</param>
    /// <param name="eKennz">The e kennz.</param>
    /// <returns>IEnumerable&lt;ILinkData&gt;.</returns>
    public IEnumerable<ILinkData> ReadAllFams(int iFamNr, ELinkKennz eKennz = ELinkKennz.lkNone)
        => ReadLinkDataDB(LinkIndex.FamSu, eKennz == 0 ? (rs) => rs.Seek("=", iFamNr) : (rs) => rs.Seek("=", iFamNr, eKennz),
            eKennz == ELinkKennz.lkNone ? (ld) => ld.iFamNr != iFamNr : (ld) => ld.iFamNr != iFamNr || ld.eKennz != eKennz);

    /// <summary>
    /// Reads the link data from database. Using the index ElSu
    /// </summary>
    /// <param name="iPersNr">The i pers nr.</param>
    /// <param name="eKennz">The e kennz.</param>
    /// <returns>IEnumerable&lt;ILinkData&gt;.</returns>
    public IEnumerable<ILinkData> ReadAllPers(int iPersNr, ELinkKennz eKennz = ELinkKennz.lkNone)
        => ReadLinkDataDB(LinkIndex.ElSu, eKennz == 0 ? (rs) => rs.Seek("=", iPersNr) : (rs) => rs.Seek("=", iPersNr, eKennz),
            eKennz == ELinkKennz.lkNone ? (ld) => ld.iPersNr != iPersNr : (ld) => ld.iPersNr != iPersNr || ld.eKennz != eKennz);
    /// <summary>
    /// Reads the link data from database. Using the index PAFI (Personen mit Familienkennzeichen)
    /// </summary>
    /// <param name="iKennz">The i kennz.</param>
    /// <returns>IEnumerable&lt;ILinkData&gt;.</returns>
    public IEnumerable<ILinkData> ReadAllKennzs(ELinkKennz iKennz)
        => ReadLinkDataDB(LinkIndex.PAFI, (rs) => rs.Seek("=", iKennz), (ld) => ld.eKennz != iKennz);

    private IEnumerable<ILinkData> ReadLinkDataDB(LinkIndex Idx, Action<IRecordset> SeekAkt, Predicate<CLinkData> StopPred)
    {
        var db_LinkTable = _db_Table;
        db_LinkTable.Index = Idx.AsString();
        SeekAkt(db_LinkTable);
        while (!db_LinkTable.NoMatch
            && !db_LinkTable.EOF)
        {
            var Link = new CLinkData
            {
                eKennz = db_LinkTable.Fields[nameof(ILinkData.LinkFields.Kennz)].AsEnum<ELinkKennz>(),
                iFamNr = db_LinkTable.Fields[nameof(ILinkData.LinkFields.FamNr)].AsInt(),
                iPersNr = db_LinkTable.Fields[nameof(ILinkData.LinkFields.PerNr)].AsInt()
            };
            if (StopPred(Link))
                break;
            yield return Link;
            db_LinkTable.MoveNext();
        }
        yield break;
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

    public override ILinkData.LinkFields GetIndex1Field(LinkIndex eIndex)
    {
        return eIndex switch
        {
            LinkIndex.FamNr => ILinkData.LinkFields.FamNr,
            LinkIndex.ElSu => ILinkData.LinkFields.PerNr,
            _ => throw new NotImplementedException(),
        };
    }

    protected override ILinkData GetData(IRecordset rs)
    {
        return new CLinkData(rs);
    }
}
