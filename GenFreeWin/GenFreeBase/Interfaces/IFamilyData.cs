using System;

namespace GenFree.Interfaces;

public interface IFamilyData : IHasID<int>
{
    DateTime dAnlDatum { get; }
    DateTime dEditDat { get; }
    int iName { get; }
    int iPrae { get; }
    int iSuf { get; }
    string sPruefen { get; }
    string[] sBem { get; }
    string sName { get; }
    string sPrefix { get; }
    string sSuffix { get; }
}