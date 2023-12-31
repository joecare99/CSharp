//using DAO;
using System;
using System.Collections.Generic;
using System.Reflection;
using GenFree.Model;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Sys;

namespace GenFree.Data;

public class CFamily : CUsesRecordSet<int>, IFamily
{
    private ISysTime _sysTime;
    private Func<IRecordset> _value;

   protected override IRecordset _db_Table => _value();

    protected override string _keyIndex => nameof(FamilyIndex.Fam);

    public CFamily(Func<IRecordset> value, ISysTime sysTime)
    {
        this._sysTime = sysTime;
        _value = value;
    }

    public void SetNameNr(int iFamInArb, int iName)
    {
        var dB_FamilyTable = Seek(iFamInArb);
        if (dB_FamilyTable.NoMatch)
        {
            dB_FamilyTable.AddNew();
            dB_FamilyTable.Fields[nameof(FamilyFields.AnlDatum)].Value = _sysTime.Now;
            dB_FamilyTable.Fields[nameof(FamilyFields.EditDat)].Value = 0;
            dB_FamilyTable.Fields[nameof(FamilyFields.FamNr)].Value = iFamInArb;
            dB_FamilyTable.Fields[nameof(FamilyFields.Bem1)].Value = " ";
            dB_FamilyTable.Fields[nameof(FamilyFields.Bem2)].Value = " ";
            dB_FamilyTable.Fields[nameof(FamilyFields.Bem3)].Value = " ";
            dB_FamilyTable.Fields[nameof(FamilyFields.Name)].Value = iName;
            dB_FamilyTable.Fields[nameof(FamilyFields.Prüfen)].Value = "1";
        }
        else
        {
            dB_FamilyTable.Edit();
            dB_FamilyTable.Fields[nameof(FamilyFields.Name)].Value = iName;
        }
        dB_FamilyTable.Update();
    }

    public void SetValue(int famInArb, int satz, string[] strings, object[] objects)
    {
        throw new NotImplementedException();
    }

    public override IRecordset Seek(int key, out bool xBreak)
    {
        _db_Table.Index = _keyIndex;
        _db_Table.Seek("=", key);
        xBreak = _db_Table.NoMatch;
        return xBreak? null: _db_Table;
    }

    public bool ReadData(int key, out IFamilyData? data)
    {
        var dB_FamilyTable = Seek(key, out bool xBreak);
        data = xBreak ? null : new CFamilyPersons(dB_FamilyTable);
        return !xBreak;
    }

    public IEnumerable<IFamilyData> ReadAll()
    {
        IRecordset dB_PlaceTable = _db_Table;
        dB_PlaceTable.Index = _keyIndex;
        dB_PlaceTable.MoveFirst();
        while (!dB_PlaceTable.EOF)
        {
            yield return new CFamilyPersons(dB_PlaceTable);
            dB_PlaceTable.MoveNext();
        }
    }

    public void SetData(int key, IFamilyData data, string[]? asProps = null)
    {
        var dB_FamilyTable = Seek(key);
        if (dB_FamilyTable != null)
        {
            dB_FamilyTable.Edit();
            data.SetDBValue(dB_FamilyTable, asProps);
            dB_FamilyTable.Update();
        }
    }

    protected override int GetID(IRecordset recordset)
    {
        return recordset.Fields[nameof(FamilyFields.FamNr)].AsInt();
    }
}

