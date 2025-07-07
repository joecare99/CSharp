//using DAO;
using GenFree.Data;
using GenFree.Interfaces.Data;
using System;

namespace GenFree.Interfaces.Model;

public interface IPlace : IHasIxDataItf<PlaceIndex,IPlaceData,int>, IUsesRecordset<int>, IUsesID<int>, IHasRSIndex1<PlaceIndex,PlaceFields>
{
    IPlaceData? CreateNew();
    void ForeEachTextDo(Func<int, string> onGetText, Action<int, string[]> onDo, Action<float, int>? onProgress = null);
    /// <summary>
    /// the fully qualified name of the place.
    /// </summary>
    /// <param name="cPlace">The place.</param>
    /// <param name="xNoPraepos">if set to <c>true</c> the prepositions are omitted.</param>
    /// <param name="xAdditional">if set to <c>true</c> the county, region, state is included.</param>
    /// <returns>the fully qualified name of the place. [string]</returns>
    string FullName(IPlaceData? cPlace, bool xNoPraepos=true, bool xAdditional=false);
    bool ReadIdxData(PlaceIndex eIdx,object value, out IPlaceData? cPlace);
}