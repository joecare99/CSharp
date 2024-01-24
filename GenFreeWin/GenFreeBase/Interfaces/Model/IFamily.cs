//using DAO;
using GenFree.Data;

namespace GenFree.Interfaces.Model
{
    public interface IFamily :IHasIxDataItf<FamilyIndex,IFamilyData,int>, IUsesRecordset<int> , IUsesID<int>, IHasRSIndex1<FamilyIndex,FamilyFields>
    {
        void SetNameNr(int iFamInArb, int iName);
        void SetValue(int famInArb, int satz, (EFamilyProp, object)[] atProps);
    }
}