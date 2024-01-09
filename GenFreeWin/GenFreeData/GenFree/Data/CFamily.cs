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

public class CFamily : CUsesIndexedRSet<int, FamilyIndex, FamilyFields, IFamilyData>, IFamily
{
    private ISysTime _sysTime;
    private Func<IRecordset> _value;

    protected override IRecordset _db_Table => _value();

    protected override FamilyIndex _keyIndex => FamilyIndex.Fam;

    public CFamily(Func<IRecordset> value, ISysTime sysTime)
    {
        this._sysTime = sysTime;
        _value = value;
    }

    public void SetNameNr(int iFamInArb, int iName)
    {
        var dB_FamilyTable = Seek(iFamInArb);
        if (dB_FamilyTable?.NoMatch != false)
        {
            dB_FamilyTable = _db_Table;
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
            dB_FamilyTable.Fields[nameof(FamilyFields.EditDat)].Value = _sysTime.Now;

        }
        dB_FamilyTable.Update();
    }

    public void SetValue(int famInArb, int satz, (EFamilyProp, object)[] atProps)
    {
        SetNameNr(famInArb, satz);
        _db_Table.Edit();
        foreach (var (eProp, oVal) in atProps)
        {
            switch (eProp)
            {
                case EFamilyProp.xAeB:
                    _db_Table.Fields[nameof(FamilyFields.Aeb)].Value = oVal;
                    break;
                default:
                    break;
            }
        }
        _db_Table.Update();
    }

    protected override int GetID(IRecordset recordset)
    {
        return recordset.Fields[nameof(FamilyFields.FamNr)].AsInt();
    }

    public override FamilyFields GetIndex1Field(FamilyIndex eIndex) => eIndex switch
    {
        FamilyIndex.Fam => FamilyFields.FamNr,
        FamilyIndex.Fuid => FamilyFields.Fuid,
        FamilyIndex.BeaDat => FamilyFields.EditDat,
        _ => throw new ArgumentException(nameof(eIndex)),
    };

    protected override IFamilyData GetData(IRecordset rs) => new CFamilyPersons(rs);
}

