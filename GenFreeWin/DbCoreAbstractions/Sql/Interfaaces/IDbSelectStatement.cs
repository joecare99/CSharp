using System.Collections.Generic;

namespace Db.Core.Abstractions.Sql.Interfaaces
{
    /// <summary>
    /// Represents an abstract select statement.
    /// </summary>
    public interface IDbSelectStatement
    {
        /// <summary>
        /// Gets the target table name.
        /// </summary>
        string Table { get; }

        /// <summary>
        /// Gets the selected fields.
        /// </summary>
        IReadOnlyList<string> Fields { get; }

        /// <summary>
        /// Gets the filter clauses.
        /// </summary>
        IReadOnlyList<IDbFilterClause> Filters { get; }

        /// <summary>
        /// Gets the optional limit.
        /// </summary>
        int? Limit { get; }

        /// <summary>
        /// Gets the optional offset.
        /// </summary>
        object? Offset { get; }
    }
}
