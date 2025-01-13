//using DAO;
using GenFree.Data;
using System;

namespace GenFree.Interfaces.Model
{
    public interface IPlace : IHasIxDataItf<PlaceIndex,IPlaceData,int>, IUsesRecordset<int>, IUsesID<int>, IHasRSIndex1<PlaceIndex,PlaceFields>
    {
        void ForeEachTextDo(Func<int, string> onGetText, Action<int, string[]> onDo, Action<float, int>? onProgress = null);
    }
}