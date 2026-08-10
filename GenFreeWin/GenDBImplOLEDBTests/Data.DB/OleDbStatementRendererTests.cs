using Db.Core.Abstractions.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace GenFree.Data.DB.Tests;

[TestClass]
public class OleDbStatementRendererTests
{
    [TestMethod]
    public void CreateQuery_WithFieldsAndFilters_RendersExpectedSql()
    {
        using var connection = new TestDbConnection();
        var renderer = new OleDbStatementRenderer(connection);
        var filters = new List<DbFilterClause>
        {
            new("Person.Id", DbFilterOperator.Equal, "@id"),
            new("DeletedAt", DbFilterOperator.IsNull)
        };

        using var command = renderer.CreateQuery("Person", new[] { "Person.Id", "Name" }, filters);

        Assert.AreEqual("SELECT [Person].[Id],[Name] FROM [Person] WHERE [Person].[Id]=@id AND [DeletedAt] IS NULL", command.CommandText);
    }

    [TestMethod]
    public void CreateQuery_WithoutFieldsAndFilters_RendersWildcard()
    {
        using var connection = new TestDbConnection();
        var renderer = new OleDbStatementRenderer(connection);

        using var command = renderer.CreateQuery(connection, "Person", null, null);

        Assert.AreEqual("SELECT * FROM [Person]", command.CommandText);
    }

    [TestMethod]
    public void CreateDelete_WithFilters_RendersExpectedSql()
    {
        using var connection = new TestDbConnection();
        var renderer = new OleDbStatementRenderer(connection);
        var filters = new List<DbFilterClause>
        {
            new("DeletedAt", DbFilterOperator.IsNull)
        };

        using var command = renderer.CreateDelete("Person", filters);

        Assert.AreEqual("DELETE FROM [Person] WHERE [DeletedAt] IS NULL", command.CommandText);
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
        public IDbCommand CreateCommand() => new TestDbCommand();
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
            var index = IndexOf(parameterName);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }
    }
}
