using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MSQBrowser.Models;

namespace MSQBrowser.Models.Interfaces
{
    public interface IDBModel
    {
        DataTable? QuerySchema(string value);
        IList<DBMetaData> QueryTable(string sTableName);
        DataTable? QueryTableData(string value);
        Task<TablePageResult> QueryTableDataPageAsync(string value, IEnumerable<DBMetaData> columns, int offset, int pageSize, CancellationToken cancellationToken = default);
    }
}
