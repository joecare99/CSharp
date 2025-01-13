using System.Collections.Generic;

namespace GenFree.Interfaces.DB
{
    public interface IDBEngine
    {
        IList<IDBWorkSpace> Workspaces { get; }

        void CompactDatabase(string v1, string v2);
        IDatabase OpenDatabase(string path, bool v2, bool v3, string v4);
    }
}
