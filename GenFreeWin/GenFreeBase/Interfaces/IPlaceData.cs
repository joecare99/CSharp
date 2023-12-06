//using DAO;
using GenFree.Data;
using GenFree.Interfaces.DB;

namespace GenFree.Interfaces
{
    public interface IPlaceData : IHasPropEnum<EPlaceProp>, IHasID<int>
    {
        void SetDBValue(IRecordset dB_FamilyTable, string[]? asProps);
    }
}