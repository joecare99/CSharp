//using DAO;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPlace : IHasDataItf<IPlaceData,int>, IUsesRecordset<int>, IUsesID<int>
    {

        void ForeEachTextDo(Func<int, string> onGetText, Action<int, string[]> onDo, Action<float, int>? onProgress = null);
    }
}