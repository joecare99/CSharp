using GenFree.Data;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Data;

public interface IFamilyData : IHasID<int>, IHasPropEnum<EFamilyProp>, IHasIRecordset
{
    DateTime dAnlDatum { get; }
    DateTime dEditDat { get; }
    int iName { get; }
    int iPrae { get; }
    int iSuf { get; }
    /// <summary>
    /// Gets the guid.
    /// </summary>
    /// <value>The guid.</value>
    Guid? gUID { get; }
    string sPruefen { get; }
    string[] sBem { get; }
    string sName { get; }
    string sPrefix { get; }
    string sSuffix { get; }
    /// <summary>
    /// Gets a value indicating whether this is an AEB (außereheliche Beziehung).
    /// </summary>
    /// <value><c>true</c> if [x ae b]; otherwise, <c>false</c>.</value>
    bool xAeB { get; }
    int iEltern { get; }
    int Mann { get; set; }
    int Frau { get; set; }
    IList<(int nr, string aTxt)> Kinder { get; }
    IArrayProxy<int> Kind { get; }
    /// <summary>
    /// Gets the i GGV (Gleichgechlechtliche Verbindung).
    /// </summary>
    /// <value>The i GGV.</value>
    int iGgv { get; }

    void CheckSetAnlDatum(IRecordset dB_FamilyTable);
    void Clear();
}