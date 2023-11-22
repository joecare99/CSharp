//using DAO;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPlace
    {
        int Count { get; }

        void ForeEachTextDo(Action<int, string[]> onDo, Action<float, int>? onProgress = null);
    }
}