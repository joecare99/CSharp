using System.Collections.Generic;
using System.Data;

namespace MdbBrowser.Models.Interfaces
{
    public interface IDBModel
    {
        IList<DBMetaData> QueryTable(string sTableName);
        DataTable? QueryTableData(string value);
    }
}