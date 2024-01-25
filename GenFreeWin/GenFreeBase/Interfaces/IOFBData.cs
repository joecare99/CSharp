using GenFree.Data;

namespace GenFree.Interfaces
{
    public interface IOFBData : IHasID<(int, string, int)>, IHasIRecordset, IHasPropEnum<EOFBProps>
    {
        int iPerNr { get; }
        string sKennz { get; }
        int iTextNr { get; }
        string sText { get; }
    }
}