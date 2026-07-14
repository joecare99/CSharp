using Db.Core.Abstractions.Sql;
using Db.Core.Abstractions.Sql.Interfaaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GenFree.Data.DB
{
    /// <summary>
    /// Renders abstract statements to OleDb-friendly SQL syntax.
    /// </summary>
    public sealed class OleDbStatementRenderer(IDbConnection dbConnection) : IDbStatementRenderer
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        /// <inheritdoc />
        public string RenderSelect(IDbSelectStatement xStatement)
        {
            if (xStatement is null)
            {
                throw new ArgumentNullException(nameof(xStatement));
            }

            var sFields = xStatement.Fields.Count == 0
                ? "*"
                : string.Join(",", xStatement.Fields.Select(QuoteIdentifier));
            var xBuilder = new StringBuilder($"SELECT {sFields} FROM {QuoteIdentifier(xStatement.Table)}");
            AppendFilters(xBuilder, xStatement.Filters);
            return xBuilder.ToString();
        }

        /// <inheritdoc />
        public string RenderInsert(IDbInsertStatement xStatement)
        {
            if (xStatement is null)
            {
                throw new ArgumentNullException(nameof(xStatement));
            }

            var sFields = string.Join(", ", xStatement.Fields.Select(xField => QuoteIdentifier(xField.Key)));
            var sValues = string.Join(", ", xStatement.Fields.Select(xField => xField.Value));
            return $"INSERT INTO {QuoteIdentifier(xStatement.Table)} ({sFields}) VALUES ({sValues})";
        }

        /// <inheritdoc />
        public string RenderUpdate(IDbUpdateStatement xStatement)
        {
            if (xStatement is null)
            {
                throw new ArgumentNullException(nameof(xStatement));
            }

            var sSet = string.Join(", ", xStatement.Fields.Select(xField => $"{QuoteIdentifier(xField.Key)}={xField.Value}"));
            var xBuilder = new StringBuilder($"UPDATE {QuoteIdentifier(xStatement.Table)} SET {sSet}");
            AppendFilters(xBuilder, xStatement.Filters);
            return xBuilder.ToString();
        }

        private static void AppendFilters(StringBuilder xBuilder, IEnumerable<IDbFilterClause>? arrFilters)
        {
            if (arrFilters == null || !arrFilters.Any())
            {
                return;
            }

            xBuilder.Append(" WHERE ");
            xBuilder.Append(string.Join(" AND ", arrFilters.Select(RenderFilter)));
        }

        private static string RenderFilter(IDbFilterClause xClause)
        {
            return xClause.Operator switch
            {
                DbFilterOperator.Equal => $"{QuoteIdentifier(xClause.Field)}={xClause.ParameterName}",
                DbFilterOperator.IsNull => $"{QuoteIdentifier(xClause.Field)} IS NULL",
                _ => throw new NotSupportedException($"Unsupported filter operator {xClause.Operator}.")
            };
        }

        private static string QuoteIdentifier(string sIdentifier)
        {
            var arrParts = sIdentifier.Split('.');
            return string.Join(".", arrParts.Select(sPart => $"[{sPart.Trim('[', ']')}]"));
        }

        public IDbCommand CreateQuery(IDbSelectStatement xStatement)
        {
            if (xStatement is null)
            {
                throw new ArgumentNullException(nameof(xStatement));
            }
            return CreateQuery(xStatement.Table, xStatement.Fields, xStatement.Filters, xStatement.Limit, xStatement.Offset);
        }

        public IDbCommand CreateQuery(string sTable, IEnumerable<string> arrFields, IEnumerable<IDbFilterClause> arrFilters, int? iLimit = null, object? offset = null)
           => CreateQuery(_dbConnection, sTable, arrFields, arrFilters, iLimit, offset);

        public IDbCommand CreateQuery(IDbConnection dbConnection, string sTable, IEnumerable<string>? arrFields = null, IEnumerable<IDbFilterClause>? arrFilters = null, int? iLimit = null, object? offset = null)
        {
            var sFields = (arrFields?.Count() ?? 0) == 0
            ? "*"
            : string.Join(",", arrFields.Select(QuoteIdentifier));
            var xBuilder = new StringBuilder($"SELECT {sFields} FROM {QuoteIdentifier(sTable)}");
            AppendFilters(xBuilder, arrFilters);

            var xCommand = dbConnection.CreateCommand();
            xCommand.CommandText = xBuilder.ToString();
            return xCommand;
        }

        public IDbCommand CreateInsert(string sTable, IEnumerable<KeyValuePair<string, string>> arrFields)
        {
            var sFields = string.Join(", ", arrFields.Select(xField => QuoteIdentifier(xField.Key)));
            var sValues = string.Join(", ", arrFields.Select(xField => xField.Value));

            var xCommand = dbConnection.CreateCommand();
            xCommand.CommandText = $"INSERT INTO {QuoteIdentifier(sTable)} ({sFields}) VALUES ({sValues})";
            return xCommand;
        }

        public IDbCommand CreateUpdate(string sTable, IEnumerable<KeyValuePair<string, string>> arrFields, IEnumerable<DbFilterClause> arrFilters)
        {
            var sSet = string.Join(", ", arrFields.Select(xField => $"{QuoteIdentifier(xField.Key)}={xField.Value}"));
            var xBuilder = new StringBuilder($"UPDATE {QuoteIdentifier(sTable)} SET {sSet}");
            AppendFilters(xBuilder, arrFilters);

            var xCommand = dbConnection.CreateCommand();
            xCommand.CommandText = xBuilder.ToString();
            return xCommand;
        }
    }
}
