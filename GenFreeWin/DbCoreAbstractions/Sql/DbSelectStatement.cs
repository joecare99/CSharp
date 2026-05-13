using Db.Core.Abstractions.Sql.Interfaaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Db.Core.Abstractions.Sql
{
    /// <summary>
    /// Provides a concrete select statement implementation for provider-neutral SQL rendering.
    /// </summary>
    public sealed class DbSelectStatement : IDbSelectStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbSelectStatement"/> class.
        /// </summary>
        /// <param name="table">The source table name.</param>
        /// <param name="fields">The requested field list.</param>
        /// <param name="filters">The filter clauses.</param>
        /// <param name="limit">The optional row limit.</param>
        /// <param name="offset">The optional row offset.</param>
        public DbSelectStatement(string table, IEnumerable<string>? fields = null, IEnumerable<IDbFilterClause>? filters = null, int? limit = null, object? offset = null)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentException("A table name is required.", nameof(table));
            }

            Table = table;
            Fields = fields?.ToArray() ?? Array.Empty<string>();
            Filters = filters?.ToArray() ?? Array.Empty<IDbFilterClause>();
            Limit = limit;
            Offset = offset;
        }

        /// <inheritdoc />
        public string Table { get; }

        /// <inheritdoc />
        public IReadOnlyList<string> Fields { get; }

        /// <inheritdoc />
        public IReadOnlyList<IDbFilterClause> Filters { get; }

        /// <inheritdoc />
        public int? Limit { get; }
        /// <inheritdoc />
        public object? Offset { get; }
    }
}
