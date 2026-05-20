using System.Collections.Generic;

namespace Db.Core.Abstractions.Sql.Interfaaces
{
    /// <summary>
    /// Represents an abstract insert statement.
    /// </summary>
    public interface IDbInsertStatement
    {
        /// <summary>
        /// Gets the target table name.
        /// </summary>
        string Table { get; }

        /// <summary>
        /// Gets the inserted fields and their parameter names.
        /// </summary>
        IReadOnlyList<KeyValuePair<string, string>> Fields { get; }
    }
}
