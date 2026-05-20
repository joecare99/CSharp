using System.Collections.Generic;

namespace Db.Core.Abstractions.Sql.Interfaaces
{
    /// <summary>
    /// Represents an abstract update statement.
    /// </summary>
    public interface IDbUpdateStatement
    {
        /// <summary>
        /// Gets the target table name.
        /// </summary>
        string Table { get; }

        /// <summary>
        /// Gets the updated fields and their parameter names.
        /// </summary>
        IReadOnlyList<KeyValuePair<string, string>> Fields { get; }

        /// <summary>
        /// Gets the filter clauses.
        /// </summary>
        IReadOnlyList<IDbFilterClause> Filters { get; }
    }
}
