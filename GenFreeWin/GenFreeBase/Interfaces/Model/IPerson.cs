//using DAO;
using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Model;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPerson : IHasDataItf<IPersonData,int>, IUsesRecordset<int>, IUsesID<int>, IHasRSIndex1<PersonIndex,PersonFields>
    {
        void AllSetEditDate();
        int CheckID(int iPerson, bool xIgnoreSex, ELinkKennz kennz);
        string GetSex(int persInArb);
        int ValidateID<T>(int persInArb, short schalt, int MaxPersID, T tOKRes, Func<int, T> uQuery) where T : Enum;
    }
}