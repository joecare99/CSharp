//using DAO;
namespace GenFree.Interfaces.Model
{
    public interface IFamily :IHasDataItf<IFamilyData,int>, IUsesRecordset<int> , IUsesID<int>
    {
        void SetNameNr(int iFamInArb, int iName);
        void SetValue(int famInArb, int satz, string[] strings, object[] objects);
    }
}