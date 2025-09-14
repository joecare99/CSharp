//using DAO;
using GenFree.Data;
using GenFree.Interfaces.Data;

namespace GenFree.Interfaces.Model;

public interface IRepository: IHasIxDataItf<RepoIndex, IRepoData, int>, IUsesRecordset<int>, IUsesID<int>, IHasRSIndex1<RepoIndex, RepoFields>
{

}