using GenFree.Data;
using GenFree.Interfaces.Data;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model
{
    public interface ISourceLink :
        IHasIxDataItf<SourceLinkIndex, ISourceLinkData, (short, int, EEventArt, short)>,
        IUsesRecordset<(short, int, EEventArt, short)>,
        IUsesID<(short, int, EEventArt, short)>,
        IHasRSIndex1<SourceLinkIndex, SourceLinkFields>
    {
        bool Exists(int v, int famInArb, EEventArt eArt, short lfNR=0);
        /// <summary>
        /// Exists the entity.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="iFamInArb">The i fam in arb.</param>
        /// <returns><c>true</c> if the entity exists, <c>false</c> otherwise.</returns>
        bool ExistsEnt(short v, int iFamInArb);
        IEnumerable<ISourceLinkData> ReadAll(int persInArb, EEventArt eEventArt);
    }
}