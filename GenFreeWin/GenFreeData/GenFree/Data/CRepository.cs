//using DAO;
using BaseLib.Helper;
using GenFree.Models;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Data;

public class CRepository : CUsesIndexedRSet<int, RepoIndex, RepoFields, IRepoData>, IRepository
{
    private readonly Func<IRecordset> _value;
    public CRepository(Func<IRecordset> value) 
        => _value = value;

    /// <summary>Get the table of the repository</summary>
    protected override IRecordset _db_Table 
        => _value();

    /// <summary>Get the index of the repository</summary>
    protected override RepoIndex _keyIndex 
        => RepoIndex.Nr;

    
    /// <summary>
    /// Die Klasse CRepository verwaltet den Zugriff auf das Repository-Recordset und stellt Methoden zur Datenmanipulation bereit.
    /// </summary>
    /// <param name="rs"></param>
    /// <returns></returns>
    protected override int GetID(IRecordset rs) 
        => rs.Fields[RepoFields.Nr].AsInt();

    
    /// <summary>
    /// Diese Klasse implementiert das Repository-Muster für den Zugriff auf das Repository-Recordset und stellt Methoden zur Datenmanipulation bereit.
    /// </summary>
    /// <param name="rs"></param>
    /// <param name="xNoInit"></param>
    /// <returns></returns>
    protected override IRepoData GetData(IRecordset rs, bool xNoInit = false) 
        => new CRepoData(rs,xNoInit);

    /// <summary>
    /// Diese Methode gibt das Feld zurück, das für den angegebenen Index als Indexfeld verwendet wird.
    /// </summary>
    public override RepoFields GetIndex1Field(RepoIndex eIndex) 
        => RepoFields.Nr;
}