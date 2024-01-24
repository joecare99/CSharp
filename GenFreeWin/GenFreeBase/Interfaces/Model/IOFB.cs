using GenFree.Data;

namespace GenFree.Interfaces.Model
{
    public interface IOFB :
        IHasIxDataItf<OFBIndex, IOFBData, (int, string, int)>,
        IUsesRecordset<(int, string, int)>,
        IUsesID<(int, string, int)>,
        IHasRSIndex1<OFBIndex, OFBFields>
    {
    }
}