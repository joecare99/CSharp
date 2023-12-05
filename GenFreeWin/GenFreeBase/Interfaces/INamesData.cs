//using DAO;
using GenFree.Data;
using GenFree.Interfaces.DB;

namespace GenFree.Interfaces
{
    public interface INamesData : IHasPropEnum<ENamesProp>, IHasID<(int, ETextKennz, int)>
    {
        void SetDBValue(IRecordset dB_NamesTable, string[]? asProps);
    }
}