using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Model
{
    public class CSourceLink : CUsesIndexedRSet<(int, EEventArt, int), SourceLinkIndex, SourceLinkFields, ISourceLinkData>, ISourceLink
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

        protected override ISourceLinkData GetData(IRecordset rs) => new CSourceLinkData(rs);

        public override IRecordset? Seek((int, EEventArt, int) tKey, out bool xBreak)
        {
            var db_Table = _db_Table;
            db_Table.Index = _keyIndex.AsFld();
            db_Table.Seek("=", tKey.Item1, tKey.Item2, tKey.Item3);
            xBreak = db_Table.NoMatch;
            return xBreak ? null : db_Table;
        }

        protected override (int, EEventArt, int) GetID(IRecordset recordset)
            => (recordset.Fields[SourceLinkFields._1.AsFld()].AsInt(),
                                   recordset.Fields[SourceLinkFields.Art.AsFld()].AsEnum<EEventArt>(),
                                                      recordset.Fields[SourceLinkFields._2.AsFld()].AsInt());

        public IEnumerable<ISourceLinkData> ReadAll(int persInArb, EEventArt eEventArt)
        {
            IRecordset dB_SourceLinkTable = _db_Table;
            dB_SourceLinkTable.Index = _keyIndex.AsFld();
            dB_SourceLinkTable.Seek("=", 3, persInArb, eEventArt, 0);
            while (!dB_SourceLinkTable.EOF)
            {
                var Src = new CSourceLinkData(dB_SourceLinkTable);
                if (!dB_SourceLinkTable.NoMatch
                    && EEventArt.eA_Unknown != Src.eArt)
                {
                    if (Src.iLinkType != 3 // Event
                        || Src.iPersNr > persInArb
                        || Src.eArt != eEventArt
                        || Src.iLfdNr != 0)
                        break;
                    yield return Src;
                }
                dB_SourceLinkTable.MoveNext();
            }
            yield break;
        }
    }

}
