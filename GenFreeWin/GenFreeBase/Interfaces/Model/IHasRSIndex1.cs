//using DAO;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IHasRSIndex1<T,T2> where T2 : Enum where T : Enum
    {
        IRecordset? Seek(T eIndex, object oIndexVal);

        bool Exists(T eIndex, object oIndexVal);

        T2 GetIndex1Field(T eIndex);
    }
}