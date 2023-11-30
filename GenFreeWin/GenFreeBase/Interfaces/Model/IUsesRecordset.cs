//using DAO;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IUsesRecordset<T> : IUsesID<T>
    {
        int Count { get; }

        void ForEachDo(Action<IRecordset> action);
        IRecordset? Seek(T key, out bool xBreak);
        IRecordset? Seek(T Key);
    }
}