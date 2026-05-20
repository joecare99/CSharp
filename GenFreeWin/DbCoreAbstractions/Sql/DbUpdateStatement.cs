using Db.Core.Abstractions.Sql.Interfaaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Db.Core.Abstractions.Sql
{
    /// <summary>
    /// Provides a concrete update statement implementation for provider-neutral SQL rendering.
    /// </summary>
    public sealed class DbUpdateStatement : IDbUpdateStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbUpdateStatement"/> class.
        /// </summary>
        /// <param name="table">The source table name.</param>
        /// <param name="fields">The requested field list.</param>
        /// <param name="filters">The filter clauses.</param>
        /// <param name="limit">The optional row limit.</param>
        public DbUpdateStatement(string table, IEnumerable<KeyValuePair<string, string>>? fields = null, IEnumerable<IDbFilterClause>? filters = null, int? limit = null)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentException("A table name is required.", nameof(table));
            }

            Table = table;
            Fields = fields?.ToArray() ?? Array.Empty<KeyValuePair<string, string>>();
            Filters = filters?.ToArray() ?? Array.Empty<IDbFilterClause>();
            Limit = limit;
        }

        /// <inheritdoc />
        public string Table { get; }

        /// <inheritdoc />
        public IReadOnlyList<KeyValuePair<string, string>> Fields { get; }

        /// <inheritdoc />
        public IReadOnlyList<IDbFilterClause> Filters { get; }

        /// <inheritdoc />
        public int? Limit { get; }
    }
}
