//using DAO;
using GenFree.Data;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPerson
    {
        int Count { get; }

        void AllSetEditDate();
        int CheckID(int iPerson, bool xIgnoreSex, ELinkKennz kennz);
        void Delete(int persInArb);
        bool Exists(int iPersNr);
        void ForEachDo(Action<IRecordset> action);
    }
}