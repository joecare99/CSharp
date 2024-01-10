using GenFree.Data;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces;

public interface IFamilyData : IHasID<int>, IHasPropEnum<EFamilyProp>, IHasIRecordset
{
    DateTime dAnlDatum { get; }
    DateTime dEditDat { get; }
    int iName { get; }
    int iPrae { get; }
    int iSuf { get; }
    Guid? gUID { get; }
    string sPruefen { get; }
    string[] sBem { get; }
    string sName { get; }
    string sPrefix { get; }
    string sSuffix { get; }
    bool xAeB { get; }
    int iEltern { get; }
}