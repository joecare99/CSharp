using GenFree.Data;

namespace GenFree.Interfaces.Data
{
    public interface ISourceLinkData : IHasID<(short, int, EEventArt, short)>, IHasPropEnum<ESourceLinkProp>, IHasIRecordset
    {
        short iLinkType { get; }
        EEventArt eArt { get; }
        int iPerFamNr { get; }
        short iLfdNr { get; }
        int iQuNr { get; }
        string sPage { get; }
        string sEntry { get; }
        string sComment { get; }
        string sOriginalText { get; }
    }
}