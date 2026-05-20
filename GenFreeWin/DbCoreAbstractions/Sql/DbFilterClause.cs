using Db.Core.Abstractions.Sql.Interfaaces;

namespace Db.Core.Abstractions.Sql
{
    /// <summary>
    /// Provides a concrete filter clause implementation for provider-neutral SQL rendering.
    /// </summary>
    public sealed class DbFilterClause : IDbFilterClause
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbFilterClause"/> class.
        /// </summary>
        /// <param name="field">The filtered field name.</param>
        /// <param name="operator">The comparison operator.</param>
        /// <param name="parameterName">The optional parameter placeholder.</param>
        public DbFilterClause(string field, DbFilterOperator @operator, string? parameterName = null)
        {
            Field = field;
            Operator = @operator;
            ParameterName = parameterName;
        }

        /// <inheritdoc />
        public string Field { get; }

        /// <inheritdoc />
        public DbFilterOperator Operator { get; }

        /// <inheritdoc />
        public string? ParameterName { get; }
    }
}
