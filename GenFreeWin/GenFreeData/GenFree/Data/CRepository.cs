//using DAO;
using BaseLib.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Model;
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

    protected override int GetID(IRecordset rs) => rs.Fields[nameof(RepoFields.Nr)].AsInt();

    protected override IRepoData GetData(IRecordset rs) => new CRepoData(rs);

    public override RepoFields GetIndex1Field(RepoIndex eIndex) => RepoFields.Nr;
}