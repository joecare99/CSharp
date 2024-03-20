using System;
using GenFree.Interfaces.DB;
using GenFree.Interfaces;
using GenFree.Model;
using GenFree.Helper;
using System.Collections.Generic;

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
        IRecordset? db_WitnessTable = SeekFaSu(iNr, v);
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
        return SeekZeug(persInArb, eWKennz, eEvtArt, lfNR) != null;
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

    public void DeleteAllFamPred(Func<int, bool> fncFamExists)
    {
        ForEachDo((rs) =>
           {
               if (rs.Fields[nameof(WitnessFields.Art)].AsEnum<EEventArt>() > EEventArt.eA_499
                && fncFamExists(rs.Fields[nameof(WitnessFields.FamNr)].AsInt()))
                   rs.Delete();
           });

    }

    public void Append(int perfamNr, int suchPer, int kennz1, EEventArt erArt, short lfNR)
    {
        var DB_WitnessTable = Seek((perfamNr, suchPer, kennz1, erArt, lfNR));
        if (DB_WitnessTable == null)
        {
            AppendRaw(perfamNr, suchPer, kennz1, erArt, lfNR);
        }
    }

    public void Add(int iPerfam, int personNr, EEventArt art, short lfNR, int iWKennz = 10)
        => AppendRaw(iPerfam, personNr, iWKennz, art, lfNR);

    private void AppendRaw(int perfamNr, int suchPer, int kennz1, EEventArt erArt, short lfNR)
    {
        IRecordset DB_WitnessTable = _db_Table;
        DB_WitnessTable.AddNew();
        DB_WitnessTable.Fields[nameof(WitnessFields.PerNr)].Value = suchPer;
        DB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].Value = perfamNr;
        DB_WitnessTable.Fields[nameof(WitnessFields.Kennz)].Value = kennz1;
        DB_WitnessTable.Fields[nameof(WitnessFields.Art)].Value = erArt;
        DB_WitnessTable.Fields[nameof(WitnessFields.LfNr)].Value = lfNR;
        DB_WitnessTable.Update();
    }

    public override IRecordset? Seek((int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr) tValue, out bool xBreak)
    {
        var dB_Table = _db_Table;
        dB_Table.Index = $"{_keyIndex}";
        dB_Table.Seek("=", tValue.iLink, tValue.iPers, tValue.iWKennz, tValue.eArt, tValue.iLfNr);
        xBreak = dB_Table.NoMatch;
        return xBreak ? null : dB_Table;
    }

    private IRecordset? SeekFaSu(int iLink, int iWKennz)
    {
        _db_Table.Index = nameof(WitnessIndex.FamSu);
        _db_Table.Seek("=", iLink, iWKennz);
        return _db_Table.NoMatch ? null : _db_Table;
    }

    private IRecordset? SeekZeug(int persInArb, int sWKennz, EEventArt eArt, short iLfNr)
    {
        _db_Table.Index = nameof(WitnessIndex.ZeugSu);
        _db_Table.Seek("=", persInArb, sWKennz, eArt, iLfNr);
        return _db_Table.NoMatch ? null : _db_Table;
    }

    public void UpdateAllReplFams(int Fam1, int Fam2)
    {
        var eWKennz = 10;
        IRecordset? dB_WitnessTable = SeekFaSu(Fam1, eWKennz);
        while (dB_WitnessTable?.EOF == false
            && !dB_WitnessTable.NoMatch
            && !(dB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].AsInt() != Fam1))
        {
            dB_WitnessTable.Edit();
            dB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].Value = Fam2;
            dB_WitnessTable.Update();
            dB_WitnessTable.MoveNext();
        }
    }


    public void UpdateAllReplFams(int Fam1, int Fam2, short iLfNr2, EEventArt eArt2)
    {
        var eWKennz = 10;
        IRecordset dB_WitnessTable = SeekFaSu(Fam1, eWKennz);
        while (dB_WitnessTable?.EOF == false
            && !dB_WitnessTable.NoMatch
            && !(dB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].AsInt() != Fam1))
        {
            dB_WitnessTable.Edit();
            dB_WitnessTable.Fields[nameof(WitnessFields.FamNr)].Value = Fam2;
            dB_WitnessTable.Fields[nameof(WitnessFields.Art)].Value = eArt2;
            dB_WitnessTable.Fields[nameof(WitnessFields.LfNr)].Value = iLfNr2;
            dB_WitnessTable.Update();
            dB_WitnessTable.MoveNext();
        }
    }

}
