//using DAO;
using GenFree.Data;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPerson : IUsesRecordset<int>, IHasID<int>
    {
        void AllSetEditDate();
        int CheckID(int iPerson, bool xIgnoreSex, ELinkKennz kennz);
        string GetSex(int persInArb);
        int ValidateID<T>(int persInArb, short schalt, int MaxPersID, T tOKRes, Func<int, T> uQuery) where T : Enum;
    }
}