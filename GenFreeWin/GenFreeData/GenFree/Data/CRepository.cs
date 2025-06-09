//using DAO;
using BaseLib.Helper;
using GenFree.GenFree.Model;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Data;

internal class CRepository : CUsesIndexedRSet<int, RepoIndex, RepoFields, IRepoData>, IRepository
{
    private readonly Func<IRecordset> _value;
    public CRepository(Func<IRecordset> value)
    {
        _value = value;
    }
    protected override IRecordset _db_Table => _value();

    protected override RepoIndex _keyIndex => RepoIndex.Nr;

    protected override int GetID(IRecordset rs) => rs.Fields[RepoFields.Nr].AsInt();

    protected override IRepoData GetData(IRecordset rs, bool xNoInit = false) => new CRepoData(rs,xNoInit);

    public override RepoFields GetIndex1Field(RepoIndex eIndex) => RepoFields.Nr;
}