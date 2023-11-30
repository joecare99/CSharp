//using DAO;
namespace GenFree.Interfaces.Model
{
    public interface IFamily :IUsesRecordset<int> , IHasID<int>
    {
        int Count { get; }
        int iMaxFamNr { get; }

        void SetNameNr(int iFamInArb, int iName);
        void SetValue(int famInArb, int satz, string[] strings, object[] objects);
    }
}