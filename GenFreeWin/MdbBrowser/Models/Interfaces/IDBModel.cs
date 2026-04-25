using System.Collections.Generic;
using System.Data;

namespace MdbBrowser.Models.Interfaces
{
    public interface IDBModel
    {
        DataTable? QuerySchema(string value);
        IList<DBMetaData> QueryTable(string sTableName);
        DataTable? QueryTableData(string value);
    }
}