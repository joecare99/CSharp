using GenFree.Data;

namespace GenFree.Interfaces.Data
{
    public interface ISourceLinkData : IHasID<(int, EEventArt, int, short)>, IHasPropEnum<ESourceLinkProp>, IHasIRecordset
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