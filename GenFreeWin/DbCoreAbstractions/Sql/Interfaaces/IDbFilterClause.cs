namespace Db.Core.Abstractions.Sql.Interfaaces
{
    /// <summary>
    /// Represents a single abstract WHERE clause filter.
    /// </summary>
    public interface IDbFilterClause
    {
        /// <summary>
        /// Gets the filtered field name.
        /// </summary>
        string Field { get; }

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        DbFilterOperator Operator { get; }

        /// <summary>
        /// Gets the optional parameter name.
        /// </summary>
        string? ParameterName { get; }
    }
}
