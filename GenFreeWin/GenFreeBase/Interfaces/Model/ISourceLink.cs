using GenFree.Data;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model
{
    public interface ISourceLink :
        IHasIxDataItf<SourceLinkIndex, ISourceLinkData, (int, EEventArt, int)>,
        IUsesRecordset<(int, EEventArt, int)>,
        IUsesID<(int, EEventArt, int)>,
        IHasRSIndex1<SourceLinkIndex, SourceLinkFields>
    {
        IEnumerable<ISourceLinkData> ReadAll(int persInArb, EEventArt eEventArt);
    }
}