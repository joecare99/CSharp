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
        string GetSex(int persInArb);
        int iMaxPersNr();
        IRecordset SeekPerson(int persInArb, out bool xBreak);
        IRecordset SeekPerson(int persInArb);
        int ValidateID<T>(int persInArb, short schalt, int MaxPersID, T tOKRes, Func<int, T> uQuery) where T : Enum;
    }
}