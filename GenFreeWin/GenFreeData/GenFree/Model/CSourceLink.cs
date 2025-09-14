using BaseLib.Helper;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;
using System.Collections.Generic;

namespace GenFree.Models;

public class CSourceLink : CUsesIndexedRSet<(short, int, EEventArt, short), SourceLinkIndex, SourceLinkFields, ISourceLinkData>, ISourceLink
{
    private Func<IRecordset> _value;

    public CSourceLink(Func<IRecordset> value) => _value = value;

    protected override SourceLinkIndex _keyIndex => SourceLinkIndex.Tab22;

    protected override IRecordset _db_Table => _value();

    public override SourceLinkFields GetIndex1Field(SourceLinkIndex eIndex) 
        => eIndex switch
    {
        SourceLinkIndex.Tab => SourceLinkFields._2,
        _ => throw new ArgumentException()
    };

    protected override ISourceLinkData GetData(IRecordset rs, bool xNoInit = false) 
        => new CSourceLinkData(rs, xNoInit);

    public override IRecordset? Seek((short, int, EEventArt, short) tKey, out bool xBreak)
    {
        var db_Table = _db_Table;
        db_Table.Index = _keyIndex.AsFld();
        db_Table.Seek("=", tKey.Item1, tKey.Item2, (int)tKey.Item3,tKey.Item4);
        xBreak = db_Table.NoMatch;
        return xBreak ? null : db_Table;
    }

    protected override (short, int, EEventArt, short) GetID(IRecordset recordset)
        => ((short)recordset.Fields[SourceLinkFields._1].AsInt(),
            recordset.Fields[SourceLinkFields._2].AsInt(),
            recordset.Fields[SourceLinkFields.Art].AsEnum<EEventArt>(),
            (short)recordset.Fields[SourceLinkFields.LfNr].AsInt());

    public IEnumerable<ISourceLinkData> ReadAll(int persInArb, EEventArt eEventArt)
    {
        IRecordset dB_SourceLinkTable = _db_Table;
        dB_SourceLinkTable.Index = _keyIndex.AsFld();
        dB_SourceLinkTable.Seek("=", 3, persInArb, eEventArt, 0);
        while (!dB_SourceLinkTable.EOF)
        {
            var Src = GetData(dB_SourceLinkTable);
            if (!dB_SourceLinkTable.NoMatch
                && EEventArt.eA_Unknown != Src.eArt)
            {
                if (Src.iLinkType != 3 // Event
                    || Src.iPerFamNr > persInArb
                    || Src.eArt != eEventArt
                    || Src.iLfdNr != 0)
                    break;
                yield return Src;
            }
            dB_SourceLinkTable.MoveNext();
        }
        yield break;
    }

    public bool Exists(int iCitKenn, int iPerFamNr, EEventArt eArt, short lfNR = 0) 
        => Exists(( (short)iCitKenn, iPerFamNr, eArt, lfNR));

    public bool ExistsEnt(short v, int iFamInArb)
    {
        IRecordset dB_SourceLinkTable = _db_Table;
        dB_SourceLinkTable.Index = SourceLinkIndex.Tab.AsFld();
        dB_SourceLinkTable.Seek("=", v, iFamInArb);
        return !dB_SourceLinkTable.NoMatch;
    }
}
