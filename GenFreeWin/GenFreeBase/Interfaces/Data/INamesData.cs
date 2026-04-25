//using DAO;

//using DAO;
using GenFree.Data;

namespace GenFree.Interfaces.Data
{
    public interface INamesData : IHasPropEnum<ENamesProp>, IHasID<(int, ETextKennz, int)>, IHasIRecordset
    {
        int iPersNr { get; }
        ETextKennz eTKennz { get; }
        int iTextNr { get; }
        int iLfNr { get; }
        bool bRuf { get; }
        bool bSpitz { get; }
    }
}