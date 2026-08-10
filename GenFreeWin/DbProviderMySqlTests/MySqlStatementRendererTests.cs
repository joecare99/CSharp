using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Db.Core.Abstractions.Sql;
using Db.Core.Abstractions.Sql.Interfaaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Db.Provider.MySql.Tests
{
    [TestClass]
    public class MySqlStatementRendererTests
    {
        [TestMethod]
        public void CreateQuery_WithFieldsFiltersAndOffset_RendersExpectedSql()
        {
            using var xConnection = new TestDbConnection();
            var xRenderer = new MySqlStatementRenderer(xConnection);
            var arrFilters = new List<IDbFilterClause>
            {
                new DbFilterClause("Person.Id", DbFilterOperator.Equal, "@id"),
                new DbFilterClause("DeletedAt", DbFilterOperator.IsNull)
            };

            using var xCommand = xRenderer.CreateQuery("Person", new[] { "Person.Id", "Name" }, arrFilters, 5, "@offset");

            Assert.AreEqual("SELECT `Person`.`Id`,`Name` FROM `Person` WHERE `Person`.`Id`=@id AND `DeletedAt` is null limit 5 offset @offset", xCommand.CommandText);
        }

        [TestMethod]
        public void CreateQuery_WithWildcardAndNoFilters_RendersExpectedSql()
        {
            using var xConnection = new TestDbConnection();
            var xRenderer = new MySqlStatementRenderer(xConnection);

            using var xCommand = xRenderer.CreateQuery(xConnection, "Person", new[] { "*" }, Array.Empty<IDbFilterClause>());

            Assert.AreEqual("SELECT * FROM `Person`", xCommand.CommandText);
        }

        [TestMethod]
        public void CreateDelete_WithFilters_RendersExpectedSql()
        {
            using var xConnection = new TestDbConnection();
            var xRenderer = new MySqlStatementRenderer(xConnection);
            var arrFilters = new List<DbFilterClause>
            {
                new("DeletedAt", DbFilterOperator.IsNull)
            };

            using var xCommand = xRenderer.CreateDelete("Person", arrFilters);

            Assert.AreEqual("DELETE FROM `Person` WHERE `DeletedAt` is null", xCommand.CommandText);
        }

        private sealed class TestDbConnection : IDbConnection
        {
            public string? ConnectionString { get; set; }
            public int ConnectionTimeout { get; set; }
            public string? Database { get; set; }
            public ConnectionState State { get; set; }

            public IDbTransaction BeginTransaction() => throw new NotSupportedException();
            public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotSupportedException();
            public void ChangeDatabase(string databaseName) => Database = databaseName;
            public void Close() => State = ConnectionState.Closed;
            public IDbCommand CreateCommand() => new TestDbCommand { Connection = this };
            public void Open() => State = ConnectionState.Open;
            public void Dispose() => State = ConnectionState.Closed;
        }

        private sealed class TestDbCommand : IDbCommand
        {
            public string? CommandText { get; set; }
            public int CommandTimeout { get; set; }
            public CommandType CommandType { get; set; }
            public IDbConnection? Connection { get; set; }
            public IDataParameterCollection Parameters { get; } = new TestParameterCollection();
            public IDbTransaction? Transaction { get; set; }
            public UpdateRowSource UpdatedRowSource { get; set; }

            public void Cancel() { }
            public IDbDataParameter CreateParameter() => new TestDbParameter();
            public int ExecuteNonQuery() => 0;
            public IDataReader ExecuteReader() => throw new NotSupportedException();
            public IDataReader ExecuteReader(CommandBehavior behavior) => throw new NotSupportedException();
            public object? ExecuteScalar() => null;
            public void Prepare() { }
            public void Dispose() { }
        }

        private sealed class TestDbParameter : IDbDataParameter
        {
            public DbType DbType { get; set; }
            public ParameterDirection Direction { get; set; }
            public bool IsNullable { get; set; }
            public string? ParameterName { get; set; }
            public string? SourceColumn { get; set; }
            public DataRowVersion SourceVersion { get; set; }
            public object? Value { get; set; }
            public byte Precision { get; set; }
            public byte Scale { get; set; }
            public int Size { get; set; }
        }

        private sealed class TestParameterCollection : ArrayList, IDataParameterCollection
        {
            public object this[string parameterName]
            {
                get => this[IndexOf(parameterName)];
                set => this[IndexOf(parameterName)] = value;
            }

            public bool Contains(string parameterName) => IndexOf(parameterName) >= 0;

            public int IndexOf(string parameterName)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (this[i] is IDataParameter parameter && string.Equals(parameter.ParameterName, parameterName, StringComparison.Ordinal))
                    {
                        return i;
                    }
                }

                return -1;
            }

            public void RemoveAt(string parameterName)
            {
                var iIndex = IndexOf(parameterName);
                if (iIndex >= 0)
                {
                    RemoveAt(iIndex);
                }
            }
        }
    }
}
