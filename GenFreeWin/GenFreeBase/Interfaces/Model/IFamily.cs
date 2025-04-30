﻿//using DAO;
using GenFree.Data;
using GenFree.Interfaces.Data;

namespace GenFree.Interfaces.Model
{
    public interface IFamily :IHasIxDataItf<FamilyIndex,IFamilyData,int>, IUsesRecordset<int> , IUsesID<int>, IHasRSIndex1<FamilyIndex,FamilyFields>
    {
        void AllSetEditDate();
        void AppendRaw(int iFamNr, int iName, int iAeb, string sBem1);
        void SetNameNr(int iFamInArb, int iName);
        void SetValue(int famInArb, int satz, (EFamilyProp, object)[] atProps);
    }
}