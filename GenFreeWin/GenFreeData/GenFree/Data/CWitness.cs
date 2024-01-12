using System;
using System.Linq;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces;
using GenFree.Model;
using GenFree.Helper;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace GenFree.Data;


public class CWitness : CUsesIndexedRSet<(int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr), WitnessIndex, WitnessFields, IWitnessData>, IWitness
{
    private Func<IRecordset> _value;

    public CWitness(Func<IRecordset> value)
    {
        _value = value;
    }

    protected override IRecordset _db_Table => _value();

    protected override WitnessIndex _keyIndex => WitnessIndex.Fampruef;

    protected override (int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr) GetID(IRecordset recordset)
    {
        return (_db_Table.Fields[nameof(WitnessFields.FamNr)].AsInt(),
                _db_Table.Fields[nameof(WitnessFields.PerNr)].AsInt(),
                _db_Table.Fields[nameof(WitnessFields.Kennz)].AsInt(),
                _db_Table.Fields[nameof(WitnessFields.Art)].AsEnum<EEventArt>(),
                (short)_db_Table.Fields[nameof(WitnessFields.LfNr)].AsInt());
    }

    public override WitnessFields GetIndex1Field(WitnessIndex eIndex) => eIndex switch
    {
        WitnessIndex.ElSu => WitnessFields.PerNr,
        WitnessIndex.FamSu => WitnessFields.FamNr,
        _ => throw new ArgumentException(nameof(eIndex)),
    };

    protected override IWitnessData GetData(IRecordset rs) => new CWitnessData(rs);

    public IEnumerable<IWitnessData> ReadAllFams(int iNr, int v)
    {
        IRecordset? db_WitnessTable = SeekFaSu(iNr,v);
        while (db_WitnessTable?.EOF == false
            && !db_WitnessTable.NoMatch
            && db_WitnessTable.Fields[nameof(WitnessFields.FamNr)].AsInt() == iNr
            && db_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsInt() == v)
        {
            yield return GetData(db_WitnessTable);
            db_WitnessTable.MoveNext();
        }
    }

    public bool ExistZeug(int persInArb, EEventArt eEvtArt, short lfNR, int eWKennz = 10)
    {
        return SeekZeug( persInArb, eWKennz, eEvtArt, lfNR) !=null;
    }

    public void DeleteAllE(int persInArb, int eWKennz)
    {
        IRecordset DB_WitnessTable = _db_Table;

        DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
        DB_WitnessTable.Seek("=", persInArb, eWKennz);
        var I = 1;
        while (I <= 99
            && !DB_WitnessTable.NoMatch
            && !DB_WitnessTable.EOF 
            && DB_WitnessTable.Fields[nameof(WitnessFields.PerNr)].AsInt() == persInArb
              && DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsInt() == eWKennz)
        {
            DB_WitnessTable.Delete();
            DB_WitnessTable.MoveNext();
            I++;
        }
    }
    public void DeleteAllF(int persInArb, int sWKennz)
    {
        IRecordset DB_WitnessTable = _db_Table;
        DB_WitnessTable.Index = nameof(WitnessIndex.FamSu);
        DB_WitnessTable.Seek("=", persInArb, sWKennz);
        var I = 1;
        while (I <= 99
            && !DB_WitnessTable.NoMatch
            && !DB_WitnessTable.EOF
            && DB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].AsInt() == persInArb
            && DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsInt() == sWKennz)
        {
            if (DB_WitnessTable.Fields[nameof(WitnessFields.Art)].AsInt() < 500)
            {
                DB_WitnessTable.Delete();
            }
            DB_WitnessTable.MoveNext();
            I++;
        }
    }

    public void DeleteAllZ(int persInArb, int sWKennz, EEventArt eArt, short iLfNr)
    {
        var DB_WitnessTable = SeekZeug(persInArb, sWKennz, eArt, iLfNr);
        while (DB_WitnessTable?.NoMatch == false
            && !DB_WitnessTable.EOF
            && DB_WitnessTable.Fields[nameof(WitnessFields.PerNr)].AsInt() == persInArb
            && DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].AsInt() == sWKennz
            && DB_WitnessTable.Fields[nameof(WitnessFields.Art)].AsEnum<EEventArt>() == eArt
            && DB_WitnessTable.Fields[nameof(WitnessFields.LfNr)].AsInt() == iLfNr)
        {
            DB_WitnessTable.Delete();
            DB_WitnessTable.MoveNext();
        }
    }

    private IRecordset? SeekFaSu(int iNr, int v)
    {
        _db_Table.Index = nameof(WitnessIndex.FamSu);
        _db_Table.Seek("=", iNr, v);
        return _db_Table.NoMatch ? null : _db_Table;
    }

    private IRecordset? SeekZeug(int persInArb, int sWKennz, EEventArt eArt, short iLfNr)
    {
        _db_Table.Index = nameof(WitnessIndex.ZeugSu);
        _db_Table.Seek("=", persInArb, sWKennz, eArt, iLfNr);
        return _db_Table.NoMatch ? null : _db_Table;
    }
}
