//using DAO;
using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;

namespace GenFree.Model
{
    public interface IHasRSIndex1<T,T2> where T2 : Enum where T : Enum
    {
        IRecordset? Seek(T eIndex, object oIndexVal);

        bool Exists(T eIndex, object oIndexVal);

        T2 GetIndex1Field(T eIndex);
    }
}