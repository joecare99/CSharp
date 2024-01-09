using GenFree.Data;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces
{
    public interface ISourceLinkData : IHasID<(int, EEventArt, int)>, IHasPropEnum<ESourceLinkProp>, IHasIRecordset
    {
        EEventArt eArt { get; }
        int iLfdNr { get; }
        int iLinkType { get; }
        int iPersNr { get; }
        int iQuNr { get; }
        string sAus { get; }
        string sField3 { get; }
        string sKom { get; }
        string sOrig { get; }
    }
}