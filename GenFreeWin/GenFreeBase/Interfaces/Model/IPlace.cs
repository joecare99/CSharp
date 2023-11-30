//using DAO;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPlace : IHasID<int>
    {
        int Count { get; }
        int iMaxNr { get; }

        void ForeEachTextDo(Action<int, string[]> onDo, Action<float, int>? onProgress = null);
    }
}