using GenFree.Data;
using GenFree.Interfaces.DB;

namespace GenFree.Interfaces.Model
{
    public interface IOFB :
        IHasIxDataItf<OFBIndex, IOFBData, (int, string, int)>,
        IUsesRecordset<(int, string, int)>,
        IUsesID<(int, string, int)>,
        IHasRSIndex1<OFBIndex, OFBFields>
    {
        bool DeleteIndNr(int persInArb, string v);
        bool Exists(OFBIndex index, int persInArb, string sKennz);
        IRecordset? SeekIndNr(int persInArb, string v, out bool xB);
        bool TextExist(int TextNr);
        void Update(string Kennz, int persInArb, int satz);
    }
}