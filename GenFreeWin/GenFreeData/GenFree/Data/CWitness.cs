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

    private IRecordset? SeekFaSu(int iNr, int v)
    {
        throw new NotImplementedException();
    }
}
