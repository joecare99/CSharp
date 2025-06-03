using GenFree.Data;
using GenFree.Interfaces.Data;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model
{
    public interface ISourceLink :
        IHasIxDataItf<SourceLinkIndex, ISourceLinkData, (int, EEventArt, int, short)>,
        IUsesRecordset<(int, EEventArt, int, short)>,
        IUsesID<(int, EEventArt, int, short)>,
        IHasRSIndex1<SourceLinkIndex, SourceLinkFields>
    {
        bool Exists(int v, int famInArb, EEventArt eArt, short lfNR=0);
        IEnumerable<ISourceLinkData> ReadAll(int persInArb, EEventArt eEventArt);
    }
}