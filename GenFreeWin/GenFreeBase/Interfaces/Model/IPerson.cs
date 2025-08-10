//using DAO;
using GenFree.Data;
using GenFree.Interfaces.Data;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPerson : IHasIxDataItf<PersonIndex, IPersonData,int>, IUsesRecordset<int>, IUsesID<int>, IHasRSIndex1<PersonIndex,PersonFields>
    {
        int MinID { get; }

        void AllSetEditDate();
        int CheckID(int iPerson, bool xIgnoreSex, ELinkKennz kennz);
        bool ExistsReligi();
        string GetSex(int persInArb);
        int ValidateID<T>(int persInArb, short schalt, int MaxPersID, T tOKRes, Func<int, T> uQuery) where T : Enum;
    }
}