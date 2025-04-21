//using DAO;
using GenFree.Data;

namespace GenFree.Interfaces
{
    public interface IRepoData : IHasPropEnum<ERepoProp>, IHasID<int>, IHasIRecordset
    {
        int ID { get; } // This is the primary key
        string sName { get; set; }
        string sOrt { get; set; }
        string sPLZ { get; set; }
        string sStrasse { get; set; }
        string sFon { get; set; }
        string sMail { get; set; }
        string sHttp { get; set; }
        string sBem { get; set; }
        string sSuchname { get; set; }
    }
}