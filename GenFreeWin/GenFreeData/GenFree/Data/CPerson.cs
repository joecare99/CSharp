//using DAO;
using System;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using System.Collections.Generic;
using GenFree.Model;

namespace GenFree.Data;
public class CPerson : CUsesIndexedRSet<int,PersonIndex,PersonFields,IPersonData>, IPerson
{
    private Func<IRecordset> _value;
    private ISysTime _sysTime;

    public CPerson(Func<IRecordset> value, ISysTime sysTime)
    {
        _value = value;
        _sysTime = sysTime;
    }

    protected override IRecordset _db_Table => _value();

    protected override PersonIndex _keyIndex => PersonIndex.PerNr;

    public void AllSetEditDate()
    {
        _db_Table.Index = $"{_keyIndex}";
        if (_db_Table.RecordCount > 0)
        {
            _db_Table.MoveFirst();
            while (!_db_Table.EOF)
            {
                if (_db_Table.Fields[nameof(PersonFields.EditDat)].AsDate() == default)
                {
                    _db_Table.Edit();
                    _db_Table.Fields[nameof(PersonFields.EditDat)].Value = _db_Table.Fields[nameof(PersonFields.AnlDatum)].Value;
                    _db_Table.Update();
                }
                _db_Table.MoveNext();
            }
        }
    }

    public int CheckID(int iPerson, bool xIgnoreSex, ELinkKennz kennz)
    {
        int Person_iMaxPersNr = MaxID;
        if (Person_iMaxPersNr < iPerson)
        {
            return -1; // Person ID to high
        }
        var dB_PersonTable = _db_Table;
        dB_PersonTable.Seek("=", iPerson);
        string Person_sSex = dB_PersonTable.Fields[nameof(PersonFields.Sex)].AsString();
        if (dB_PersonTable.NoMatch)
        {
            return -2; // Person does not exist
        }
        if (xIgnoreSex
            || (Person_sSex == "F" && kennz == ELinkKennz.lkMother)
            || (Person_sSex == "M" && kennz == ELinkKennz.lkFather))
            return 0;
        else
        {
            return -3; // Person has wrong sex.
        }
    }

    public int ValidateID<T>(int persInArb, short schalt, int MaxPersID, T tOKRes, Func<int, T> uQuery) where T : Enum
    {
        var dB_PersonTable = Seek(persInArb, out bool xBreak);
        var i = 0;
        while (xBreak
            | (dB_PersonTable?.Fields[nameof(PersonFields.Pruefen)]).AsString().Trim() == "G")
        {
            if (persInArb < 1)
            {
                persInArb = 1;
            }

            if (Count == 0 || i++ > Count)
            {
                return 0;
            }

            if (!xBreak)
            {
                if (uQuery(persInArb).Equals(tOKRes))
                {
                    dB_PersonTable!.Edit();
                    dB_PersonTable.Fields[nameof(PersonFields.Pruefen)].Value = "     ";
                    dB_PersonTable.Update();
                    xBreak = true;
                }
            }

            if (!xBreak)
                persInArb = schalt switch
                {
                    0 when persInArb > 1 => persInArb - 1,
                    0 => MaxPersID,
                    3 when persInArb < MaxPersID => persInArb + 1,
                    3 => 1,
                    _ => persInArb
                };

            if (schalt is 0 or 3)
                dB_PersonTable = Seek(persInArb, out xBreak);
        }

        return persInArb;
    }

    public string GetSex(int persInArb)
        => (Seek(persInArb, out _)?.Fields[nameof(PersonFields.Sex)]).AsString().ToUpper();


    protected override int GetID(IRecordset recordset)
        => recordset.Fields[nameof(PersonFields.PersNr)].AsInt();


    public override PersonFields GetIndex1Field(PersonIndex eIndex) => eIndex switch
    {
        PersonIndex.PerNr => PersonFields.PersNr,
        PersonIndex.Puid => PersonFields.PUid,
        PersonIndex.BeaDat => PersonFields.EditDat,
        PersonIndex.reli => PersonFields.religi,
        _ => throw new ArgumentException(nameof(eIndex)),
    };

    protected override IPersonData GetData(IRecordset rs) => new CPersonData(rs);
}
