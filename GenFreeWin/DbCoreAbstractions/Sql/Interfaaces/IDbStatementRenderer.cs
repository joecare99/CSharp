using System.Collections.Generic;
using System.Data;

namespace Db.Core.Abstractions.Sql.Interfaaces;

/// <summary>
/// Renders abstract database statements to provider-specific SQL text.
/// </summary>
public interface IDbStatementRenderer
{
    /// <summary>
    /// Renders a select statement.
    /// </summary>
    string RenderSelect(IDbSelectStatement xStatement);

    /// <summary>
    /// Renders an insert statement.
    /// </summary>
    string RenderInsert(IDbInsertStatement xStatement);

    /// <summary>
    /// Renders an update statement.
    /// </summary>
    string RenderUpdate(IDbUpdateStatement xStatement);
    IDbCommand CreateQuery(IDbSelectStatement xStatement);
    IDbCommand CreateQuery(string sTable, IEnumerable<string> arrFields, IEnumerable<IDbFilterClause> arrFilters, int? iLimit = null, object? offset = null);
    IDbCommand CreateQuery(IDbConnection dbConnection, string sTable, IEnumerable<string> arrFields, IEnumerable<IDbFilterClause> arrFilters, int? iLimit = null, object? offset = null);
    IDbCommand CreateInsert(string sTable, IEnumerable<KeyValuePair<string, string>> arrFields);
    IDbCommand CreateUpdate(string sTable, IEnumerable<KeyValuePair<string, string>> arrFields, IEnumerable<DbFilterClause> arrFilters);
}
