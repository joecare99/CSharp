﻿//using DAO;
using BaseLib.Helper;
using GenFree.GenFree.Model;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Sys;
using System;

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
            dB_FamilyTable.Fields[FamilyFields.AnlDatum].Value = _sysTime.Now;
            dB_FamilyTable.Fields[FamilyFields.EditDat].Value = 0;
            dB_FamilyTable.Fields[FamilyFields.FamNr].Value = iFamInArb;
            dB_FamilyTable.Fields[FamilyFields.Bem1].Value = " ";
            dB_FamilyTable.Fields[FamilyFields.Bem2].Value = " ";
            dB_FamilyTable.Fields[FamilyFields.Bem3].Value = " ";
            dB_FamilyTable.Fields[FamilyFields.Name].Value = iName;
            dB_FamilyTable.Fields[FamilyFields.Prüfen].Value = "1";
        }
        else
        {
            dB_FamilyTable.Edit();
            dB_FamilyTable.Fields[FamilyFields.Name].Value = iName;
            dB_FamilyTable.Fields[FamilyFields.EditDat].Value = _sysTime.Now;

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
                    _db_Table.Fields[FamilyFields.Aeb].Value = oVal;
                    break;
                default:
                    break;
            }
        }
        _db_Table.Update();
    }

    protected override int GetID(IRecordset recordset)
    {
        return recordset.Fields[FamilyFields.FamNr].AsInt();
    }

    public override FamilyFields GetIndex1Field(FamilyIndex eIndex) => eIndex switch
    {
        FamilyIndex.Fam => FamilyFields.FamNr,
        FamilyIndex.Fuid => FamilyFields.Fuid,
        FamilyIndex.BeaDat => FamilyFields.EditDat,
        _ => throw new ArgumentException(nameof(eIndex)),
    };

    protected override IFamilyData GetData(IRecordset rs, bool xNoInit = false) => new CFamilyPersons(rs,xNoInit);

    public void AllSetEditDate()
    {
        IRecordset dB_FamilyTable = _db_Table;
        dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        if (dB_FamilyTable?.RecordCount > 0)
        {
            dB_FamilyTable.MoveFirst();

            dB_FamilyTable.Seek("=", 1);
            while (!dB_FamilyTable.EOF)
            {
                if (dB_FamilyTable.Fields[FamilyFields.EditDat].AsDate() == default)
                {
                    dB_FamilyTable.Edit();
                    dB_FamilyTable.Fields[FamilyFields.EditDat].Value = dB_FamilyTable.Fields[FamilyFields.AnlDatum].Value;
                    dB_FamilyTable.Update();
                }
                dB_FamilyTable.MoveNext();
            }
        }

    }

    public void AppendRaw(int iFamNr, int iName, int iAeb, string sBem1)
    {
        IRecordset dB_FamilyTable = _db_Table;
        dB_FamilyTable.AddNew();
        dB_FamilyTable.Fields[FamilyFields.AnlDatum].Value = _sysTime.Now.ToString("yyyyMMdd");
        dB_FamilyTable.Fields[FamilyFields.EditDat].Value = _sysTime.Now.ToString("yyyyMMdd");
        dB_FamilyTable.Fields[FamilyFields.Prüfen].Value = "1    ";
        dB_FamilyTable.Fields[FamilyFields.Bem1].Value = sBem1;
        dB_FamilyTable.Fields[FamilyFields.FamNr].Value = iFamNr;
        dB_FamilyTable.Fields[FamilyFields.Name].Value = iName;
        dB_FamilyTable.Fields[FamilyFields.Aeb].Value = iAeb;
        dB_FamilyTable.Fields[FamilyFields.Fuid].Value = Guid.NewGuid();
        dB_FamilyTable.Update();
    }

    public bool Get_Aeb(int iFam)
    {
        IRecordset dB_FamilyTable = _db_Table;
        dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        dB_FamilyTable.Seek("=", iFam);
        return !dB_FamilyTable.NoMatch && dB_FamilyTable.Fields[FamilyFields.Aeb].AsBool();
    }
}

