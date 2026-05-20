using System.Collections;
using System.Data;
using System.Data.Common;
using BaseLib.Models.Interfaces;
using Db.Core.Abstractions.Sql.Interfaaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class TrauerDataRepositoryTests
{
    [TestMethod]
    public void Constructor_UsesFactoryAndOpensConnection()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        _ = xFactory.Received(1).CreateConnection(Arg.Any<IDBSettings>());
        _ = xFactory.Received(1).CreateStatementRenderer(xConnection);
        Assert.AreEqual(1, xConnection.OpenCount);
    }

    [TestMethod]
    public void BuildTrauerFallIndex_UsesQueryFactory()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderSelect(Arg.Any<IDbSelectStatement>()).Returns("SELECT `idTrauerfall`,`url` FROM `Trauerfall`");
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xConnection.ExecuteReaderResult = new FakeDbDataReader(
            ["idTrauerfall", "url"],
            [
                [3L, "url"]
            ]);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var dResult = xRepository.BuildTrauerFallIndex();

        Assert.AreEqual(1, dResult.Count);
        Assert.AreEqual(3L, dResult["url"]);
        Assert.AreEqual("SELECT `idTrauerfall`,`url` FROM `Trauerfall`", xConnection.LastCommandText);
    }

    [TestMethod]
    public void AppendTrauerFall_UsesAbstractCommand()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderInsert(Arg.Any<IDbInsertStatement>()).Returns("INSERT INTO `Trauerfall` (`URL`, `Created`, `Preread_Birth`, `Preread_Death`, `Fullname`, `Firstname`, `Lastname`, `Birthname`, `Place`, `Created_by`) VALUES (@url, @created, @birth, @death, @fullName, @firstName, @lastName, @birthName, @place, @createdBy);");
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xConnection.LastInsertedId = 77L;
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var dValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["URL"] = "u",
            ["Created"] = "c",
            ["Preread_Birth"] = "b",
            ["Preread_Death"] = "d",
            ["Fullname"] = "f",
            ["Firstname"] = "fn",
            ["Lastname"] = "ln",
            ["Birthname"] = "bn",
            ["Place"] = "p",
            ["Created_by"] = "cb"
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var iResult = xRepository.AppendTrauerFall(dValues);

        Assert.AreEqual(77L, iResult);
        Assert.AreEqual("INSERT INTO `Trauerfall` (`URL`, `Created`, `Preread_Birth`, `Preread_Death`, `Fullname`, `Firstname`, `Lastname`, `Birthname`, `Place`, `Created_by`) VALUES (@url, @created, @birth, @death, @fullName, @firstName, @lastName, @birthName, @place, @createdBy);", xConnection.LastCommandText);
        Assert.AreEqual("u", xConnection.Parameters["@url"]);
        Assert.AreEqual("c", xConnection.Parameters["@created"]);
        Assert.AreEqual(1, xConnection.ExecuteNonQueryCount);
    }

    [TestMethod]
    public void AppendTrauerFall_Returns_Zero_When_Command_Has_No_LastInsertedId_Property()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        var xConnection = new FakeDbConnectionWithoutLastInsertedId();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var dValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["URL"] = "u",
            ["Created"] = "c",
            ["Preread_Birth"] = "b",
            ["Preread_Death"] = "d",
            ["Fullname"] = "f",
            ["Firstname"] = "fn",
            ["Lastname"] = "ln",
            ["Birthname"] = "bn",
            ["Place"] = "p",
            ["Created_by"] = "cb"
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var iResult = xRepository.AppendTrauerFall(dValues);

        Assert.AreEqual(0L, iResult);
    }

    [TestMethod]
    public void AppendTrauerFall_Returns_Int_LastInsertedId_When_Command_Exposes_Int_Property()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderInsert(Arg.Any<IDbInsertStatement>()).Returns("INSERT-STUB");
        var xConnection = new IntLastInsertedIdDbConnection { IntLastInsertedId = 123 };
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var dValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["URL"] = "u",
            ["Created"] = "c",
            ["Preread_Birth"] = "b",
            ["Preread_Death"] = "d",
            ["Fullname"] = "f",
            ["Firstname"] = "fn",
            ["Lastname"] = "ln",
            ["Birthname"] = "bn",
            ["Place"] = "p",
            ["Created_by"] = "cb"
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var iResult = xRepository.AppendTrauerFall(dValues);

        Assert.AreEqual(123L, iResult);
    }

    private sealed class IntLastInsertedIdDbConnection : DbConnection
    {
        public int IntLastInsertedId { get; set; }

        public override string ConnectionString { get; set; } = string.Empty;

        public override string Database => string.Empty;

        public override string DataSource => string.Empty;

        public override string ServerVersion => string.Empty;

        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new IntLastInsertedIdDbCommand(this);
        }
    }

    private sealed class IntLastInsertedIdDbCommand : DbCommand
    {
        private readonly IntLastInsertedIdDbConnection _xConnection;
        private readonly FakeDbParameterCollection _xParameters = new();

        public IntLastInsertedIdDbCommand(IntLastInsertedIdDbConnection xConnection)
        {
            _xConnection = xConnection;
        }

        public override string CommandText { get; set; } = string.Empty;

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection? DbConnection
        {
            get => _xConnection;
            set { }
        }

        protected override DbParameterCollection DbParameterCollection => _xParameters;

        protected override DbTransaction? DbTransaction { get; set; }

        public new int LastInsertedId => _xConnection.IntLastInsertedId;

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            return 1;
        }

        public override object? ExecuteScalar()
        {
            throw new NotSupportedException();
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new FakeDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return new FakeDbDataReader([], []);
        }
    }

    private sealed class FakeDbConnection : DbConnection
    {
        public int OpenCount { get; private set; }

        public int ExecuteNonQueryCount { get; set; }

        public long LastInsertedId { get; set; }

        public string LastCommandText { get; set; } = string.Empty;

        public FakeDbDataReader? ExecuteReaderResult { get; set; }

        public Dictionary<string, object?> Parameters { get; } = new(StringComparer.Ordinal);

        public bool IsDisposed { get; private set; }

        public override string ConnectionString { get; set; } = string.Empty;

        public override string Database => string.Empty;

        public override string DataSource => string.Empty;

        public override string ServerVersion => string.Empty;

        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
            OpenCount++;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new FakeDbCommand(this);
        }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class FakeDbCommand : DbCommand
    {
        private readonly FakeDbConnection _xConnection;
        private readonly FakeDbParameterCollection _xParameters = new();

        public FakeDbCommand(FakeDbConnection xConnection)
        {
            _xConnection = xConnection;
        }

        public override string CommandText { get; set; } = string.Empty;

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection? DbConnection
        {
            get => _xConnection;
            set { }
        }

        protected override DbParameterCollection DbParameterCollection => _xParameters;

        protected override DbTransaction? DbTransaction { get; set; }

        public new long LastInsertedId => _xConnection.LastInsertedId;

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            _xConnection.LastCommandText = CommandText;
            foreach (FakeDbParameter xParameter in _xParameters)
            {
                _xConnection.Parameters[xParameter.ParameterName] = xParameter.Value;
            }

            _xConnection.ExecuteNonQueryCount++;
            return 1;
        }

        public override object? ExecuteScalar()
        {
            throw new NotSupportedException();
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new FakeDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            _xConnection.LastCommandText = CommandText;
            foreach (FakeDbParameter xParameter in _xParameters)
            {
                _xConnection.Parameters[xParameter.ParameterName] = xParameter.Value;
            }

            return _xConnection.ExecuteReaderResult ?? new FakeDbDataReader([], []);
        }
    }

    private sealed class FakeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }

        public override ParameterDirection Direction { get; set; }

        public override bool IsNullable { get; set; }

        public override string ParameterName { get; set; } = string.Empty;

        public override string SourceColumn { get; set; } = string.Empty;

        public override object? Value { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override int Size { get; set; }

        public override void ResetDbType()
        {
        }
    }

    private sealed class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _arrParameters = [];

        public override int Count => _arrParameters.Count;

        public override object SyncRoot => ((ICollection)_arrParameters).SyncRoot;

        public override int Add(object value)
        {
            _arrParameters.Add((DbParameter)value);
            return _arrParameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var xValue in values)
            {
                _ = Add(xValue!);
            }
        }

        public override void Clear()
        {
            _arrParameters.Clear();
        }

        public override bool Contains(object value)
        {
            return _arrParameters.Contains((DbParameter)value);
        }

        public override bool Contains(string value)
        {
            return _arrParameters.Any(xParameter => xParameter.ParameterName == value);
        }

        public override void CopyTo(Array array, int index)
        {
            ((ICollection)_arrParameters).CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return _arrParameters.GetEnumerator();
        }

        protected override DbParameter GetParameter(int index)
        {
            return _arrParameters[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return _arrParameters.First(xParameter => xParameter.ParameterName == parameterName);
        }

        public override int IndexOf(object value)
        {
            return _arrParameters.IndexOf((DbParameter)value);
        }

        public override int IndexOf(string parameterName)
        {
            return _arrParameters.FindIndex(xParameter => xParameter.ParameterName == parameterName);
        }

        public override void Insert(int index, object value)
        {
            _arrParameters.Insert(index, (DbParameter)value);
        }

        public override void Remove(object value)
        {
            _arrParameters.Remove((DbParameter)value);
        }

        public override void RemoveAt(int index)
        {
            _arrParameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            var iIndex = IndexOf(parameterName);
            if (iIndex >= 0)
            {
                _arrParameters.RemoveAt(iIndex);
            }
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            _arrParameters[index] = value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var iIndex = IndexOf(parameterName);
            if (iIndex >= 0)
            {
                _arrParameters[iIndex] = value;
            }
            else
            {
                _arrParameters.Add(value);
            }
        }
    }

    private sealed class FakeDbDataReader : DbDataReader
    {
        private readonly string[] _arrNames;
        private readonly object?[][] _arrRows;
        private int _iIndex = -1;

        public FakeDbDataReader(string[] arrNames, object?[][] arrRows)
        {
            _arrNames = arrNames;
            _arrRows = arrRows;
        }

        public override int FieldCount => _arrNames.Length;

        public override bool HasRows => _arrRows.Length > 0;

        public override object this[int ordinal] => _arrRows[_iIndex][ordinal]!;

        public override object this[string name] => _arrRows[_iIndex][GetOrdinal(name)]!;

        public override int Depth => 0;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        public override bool GetBoolean(int ordinal) => (bool)_arrRows[_iIndex][ordinal]!;

        public override byte GetByte(int ordinal) => (byte)_arrRows[_iIndex][ordinal]!;

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => throw new NotSupportedException();

        public override char GetChar(int ordinal) => (char)_arrRows[_iIndex][ordinal]!;

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => throw new NotSupportedException();

        public override string GetDataTypeName(int ordinal) => GetFieldType(ordinal).Name;

        public override DateTime GetDateTime(int ordinal) => (DateTime)_arrRows[_iIndex][ordinal]!;

        public override decimal GetDecimal(int ordinal) => (decimal)_arrRows[_iIndex][ordinal]!;

        public override double GetDouble(int ordinal) => (double)_arrRows[_iIndex][ordinal]!;

        public override Type GetFieldType(int ordinal) => _arrRows[_iIndex][ordinal]?.GetType() ?? typeof(object);

        public override float GetFloat(int ordinal) => (float)_arrRows[_iIndex][ordinal]!;

        public override Guid GetGuid(int ordinal) => (Guid)_arrRows[_iIndex][ordinal]!;

        public override short GetInt16(int ordinal) => (short)_arrRows[_iIndex][ordinal]!;

        public override int GetInt32(int ordinal) => (int)_arrRows[_iIndex][ordinal]!;

        public override long GetInt64(int ordinal) => (long)_arrRows[_iIndex][ordinal]!;

        public override string GetName(int ordinal) => _arrNames[ordinal];

        public override int GetOrdinal(string name) => Array.IndexOf(_arrNames, name);

        public override string GetString(int ordinal) => (string)_arrRows[_iIndex][ordinal]!;

        public override object GetValue(int ordinal) => _arrRows[_iIndex][ordinal]!;

        public override int GetValues(object[] values)
        {
            Array.Copy(_arrRows[_iIndex], values, Math.Min(values.Length, _arrNames.Length));
            return Math.Min(values.Length, _arrNames.Length);
        }

        public override bool IsDBNull(int ordinal) => _arrRows[_iIndex][ordinal] is null || _arrRows[_iIndex][ordinal] == DBNull.Value;

        public override bool NextResult() => false;

        public override bool Read()
        {
            _iIndex++;
            return _iIndex < _arrRows.Length;
        }

        public override IEnumerator GetEnumerator()
        {
            return _arrRows.GetEnumerator();
        }
    }

    [TestMethod]
    public void TrauerAnzId_Binds_Id_Parameter_And_Maps_Row()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderSelect(Arg.Any<IDbSelectStatement>()).Returns("SELECT * FROM Anzeigen WHERE idAnzeige=@id");
        var xConnection = new FakeDbConnection
        {
            ExecuteReaderResult = new FakeDbDataReader(["idAnzeige", "title"], [[5, "row"]])
        };
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var arrResult = xRepository.TrauerAnzId(5);

        Assert.AreEqual(1, arrResult.Count);
        Assert.AreEqual(5, arrResult[0]["idAnzeige"]);
        Assert.AreEqual("row", arrResult[0]["title"]);
        Assert.AreEqual("SELECT * FROM Anzeigen WHERE idAnzeige=@id", xConnection.LastCommandText);
        Assert.AreEqual(5, xConnection.Parameters["@id"]);
    }

    [TestMethod]
    public void Query_Methods_Bind_Expected_Parameters()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderSelect(Arg.Any<IDbSelectStatement>()).Returns("SELECT-STUB");
        var xConnection = new FakeDbConnection
        {
            ExecuteReaderResult = new FakeDbDataReader(["value"], [["ok"]])
        };
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        _ = xRepository.TrauerAnz(11);
        Assert.AreEqual(11, xConnection.Parameters["@announcement"]);

        _ = xRepository.LegacyTrauerAnz("A1");
        Assert.AreEqual("A1", xConnection.Parameters["@auftrag"]);

        _ = xRepository.TrauerFallEquals("url", "u", 1);
        Assert.AreEqual("u", xConnection.Parameters["@value"]);

        _ = xRepository.TrauerFallById(9);
        Assert.AreEqual(9, xConnection.Parameters["@id"]);

        _ = xRepository.TrauerFallByUrl("https://example.invalid");
        Assert.AreEqual("https://example.invalid", xConnection.Parameters["@url"]);
    }

    [TestMethod]
    public void Query_Maps_DateTime_As_Formatted_String_And_DbNull_As_Null()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderSelect(Arg.Any<IDbSelectStatement>()).Returns("SELECT-STUB");
        var dtValue = new DateTime(2024, 4, 3, 12, 13, 14);
        var xConnection = new FakeDbConnection
        {
            ExecuteReaderResult = new FakeDbDataReader(["created", "optional"], [[dtValue, DBNull.Value]])
        };
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var arrResult = xRepository.TrauerAnzIsNull("field");

        Assert.AreEqual("2024-04-03 12:13:14", arrResult[0]["created"]);
        Assert.IsNull(arrResult[0]["optional"]);
    }

    [TestMethod]
    public void AppendTrauerAnz_Binds_Announcement_Parameters_And_Returns_LastInsertId()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderInsert(Arg.Any<IDbInsertStatement>()).Returns("INSERT-STUB");
        var xConnection = new FakeDbConnection { LastInsertedId = 88 };
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var dValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["idTrauerfall"] = 1L,
            ["url"] = "u",
            ["Announcement"] = 2,
            ["release"] = new DateTime(2024, 1, 2),
            ["localpath"] = "lp",
            ["pngFile"] = "a.png",
            ["pdfFile"] = "a.pdf",
            ["Additional"] = "json",
            ["Firstname"] = "f",
            ["Lastname"] = "l",
            ["Birthname"] = "b",
            ["Birth"] = new DateTime(2001, 2, 3),
            ["Death"] = new DateTime(2024, 4, 5),
            ["Place"] = "p",
            ["Info"] = "i",
            ["ProfileImg"] = "img",
            ["Rubrik"] = 8050
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var iResult = xRepository.AppendTrauerAnz(dValues);

        Assert.AreEqual(88L, iResult);
        Assert.AreEqual("INSERT-STUB", xConnection.LastCommandText);
        Assert.AreEqual(1L, xConnection.Parameters["@idtf"]);
        Assert.AreEqual("u", xConnection.Parameters["@url"]);
        Assert.AreEqual(8050, xConnection.Parameters["@rubrik"]);
    }

    [TestMethod]
    public void AppendLegacyTAnz_Binds_Legacy_Parameters_And_Returns_LastInsertId()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderInsert(Arg.Any<IDbInsertStatement>()).Returns("INSERT-LEGACY");
        var xConnection = new FakeDbConnection { LastInsertedId = 19 };
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var dValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["Auftrag"] = "4711",
            ["url"] = "u",
            ["Announcement"] = 3,
            ["release"] = new DateTime(2024, 3, 3),
            ["localpath"] = "lp",
            ["pngFile"] = "a.png",
            ["pdfFile"] = "a.pdf",
            ["Additional"] = "json"
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var iResult = xRepository.AppendLegacyTAnz(dValues);

        Assert.AreEqual(19L, iResult);
        Assert.AreEqual("INSERT-LEGACY", xConnection.LastCommandText);
        Assert.AreEqual("4711", xConnection.Parameters["@auftrag"]);
        Assert.AreEqual("json", xConnection.Parameters["@additional"]);
    }

    [TestMethod]
    public void UpdateTrauerAnz_Returns_False_When_No_New_Rows()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var xChanged = xRepository.UpdateTrauerAnz([], []);

        Assert.IsFalse(xChanged);
        Assert.AreEqual(0, xConnection.ExecuteNonQueryCount);
    }

    [TestMethod]
    public void UpdateTrauerAnz_Returns_False_When_Id_Key_Is_Missing()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var arrNew = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["name"] = "test"
            }
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var xChanged = xRepository.UpdateTrauerAnz(arrNew, []);

        Assert.IsFalse(xChanged);
        Assert.AreEqual(0, xConnection.ExecuteNonQueryCount);
    }

    [TestMethod]
    public void UpdateTrauerAnz_Executes_Update_For_Changed_Columns()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderUpdate(Arg.Any<IDbUpdateStatement>()).Returns("UPDATE-STUB");
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var arrNew = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idAnzeige"] = 7,
                ["Info"] = "new"
            }
        };
        var arrOld = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idAnzeige"] = 7,
                ["Info"] = "old"
            }
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var xChanged = xRepository.UpdateTrauerAnz(arrNew, arrOld);

        Assert.IsTrue(xChanged);
        Assert.AreEqual(1, xConnection.ExecuteNonQueryCount);
        Assert.AreEqual("UPDATE-STUB", xConnection.LastCommandText);
        Assert.AreEqual("new", xConnection.Parameters["@value"]);
        Assert.AreEqual(7, xConnection.Parameters["@key"]);
    }

    [TestMethod]
    public void UpdateTrauerFall_Executes_Update_For_Changed_Columns()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderUpdate(Arg.Any<IDbUpdateStatement>()).Returns("UPDATE-TF");
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var arrNew = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idTrauerfall"] = 4,
                ["Place"] = "new"
            }
        };
        var arrOld = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idTrauerfall"] = 4,
                ["Place"] = "old"
            }
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        xRepository.UpdateTrauerFall(arrNew, arrOld);

        Assert.AreEqual(1, xConnection.ExecuteNonQueryCount);
        Assert.AreEqual("UPDATE-TF", xConnection.LastCommandText);
        Assert.AreEqual("new", xConnection.Parameters["@value"]);
        Assert.AreEqual(4, xConnection.Parameters["@key"]);
    }

    [TestMethod]
    public void UpdateTrauerAnz_Does_Not_Update_When_Values_Are_Unchanged()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var arrNew = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idAnzeige"] = 7,
                ["Info"] = "same"
            }
        };
        var arrOld = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idAnzeige"] = 7,
                ["Info"] = "same"
            }
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var xChanged = xRepository.UpdateTrauerAnz(arrNew, arrOld);

        Assert.IsFalse(xChanged);
        Assert.AreEqual(0, xConnection.ExecuteNonQueryCount);
    }

    [TestMethod]
    public void Dispose_Closes_Underlying_Connection()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        xRepository.Dispose();

        Assert.IsTrue(xConnection.IsDisposed);
    }

    [TestMethod]
    public void DataHandler_ConnectionFactory_Constructor_Creates_Repository_And_Disposes()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var xFile = Substitute.For<IFile>();

        using (var xHandler = new DataHandler(xFactory, new DatabaseSettings(), xFile))
        {
            Assert.IsNotNull(xHandler);
        }

        _ = xFactory.Received(1).CreateConnection(Arg.Any<IDBSettings>());
        Assert.IsTrue(xConnection.IsDisposed);
    }

    [TestMethod]
    public void TrauerAnzIsNull_And_TrauerFallIsNull_Execute_Query_Without_Parameters()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderSelect(Arg.Any<IDbSelectStatement>()).Returns("SELECT-NULL");
        var xConnection = new FakeDbConnection
        {
            ExecuteReaderResult = new FakeDbDataReader(["id"], [[1]])
        };
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        _ = xRepository.TrauerAnzIsNull("Info", 2);
        _ = xRepository.TrauerFallIsNull("Place", 3);

        Assert.AreEqual("SELECT-NULL", xConnection.LastCommandText);
    }

    [TestMethod]
    public void UpdateTrauerAnz_Skips_New_Rows_Without_Matching_Old_Id()
    {
        var xFactory = Substitute.For<IDbConnectionFactory>();
        var xRenderer = Substitute.For<IDbStatementRenderer>();
        xRenderer.RenderUpdate(Arg.Any<IDbUpdateStatement>()).Returns("UPDATE-STUB");
        var xConnection = new FakeDbConnection();
        xFactory.CreateStatementRenderer(xConnection).Returns(xRenderer);
        xFactory.CreateConnection(Arg.Any<IDBSettings>()).Returns(_ => xConnection);
        var arrNew = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idAnzeige"] = 7,
                ["Info"] = "new"
            }
        };
        var arrOld = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal)
            {
                ["idAnzeige"] = 8,
                ["Info"] = "old"
            }
        };

        using var xRepository = new TrauerDataRepository(xFactory, new DatabaseSettings());

        var xChanged = xRepository.UpdateTrauerAnz(arrNew, arrOld);

        Assert.IsFalse(xChanged);
        Assert.AreEqual(0, xConnection.ExecuteNonQueryCount);
    }

    private sealed class FakeDbConnectionWithoutLastInsertedId : DbConnection
    {
        public int OpenCount { get; private set; }

        public int ExecuteNonQueryCount { get; set; }

        public string LastCommandText { get; set; } = string.Empty;

        public Dictionary<string, object?> Parameters { get; } = new(StringComparer.Ordinal);

        public override string ConnectionString { get; set; } = string.Empty;

        public override string Database => string.Empty;

        public override string DataSource => string.Empty;

        public override string ServerVersion => string.Empty;

        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
            OpenCount++;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new FakeDbCommandWithoutLastInsertedId(this);
        }
    }

    private sealed class FakeDbCommandWithoutLastInsertedId : DbCommand
    {
        private readonly FakeDbConnectionWithoutLastInsertedId _xConnection;
        private readonly FakeDbParameterCollection _xParameters = new();

        public FakeDbCommandWithoutLastInsertedId(FakeDbConnectionWithoutLastInsertedId xConnection)
        {
            _xConnection = xConnection;
        }

        public override string CommandText { get; set; } = string.Empty;

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection? DbConnection
        {
            get => _xConnection;
            set { }
        }

        protected override DbParameterCollection DbParameterCollection => _xParameters;

        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            _xConnection.LastCommandText = CommandText;
            foreach (FakeDbParameter xParameter in _xParameters)
            {
                _xConnection.Parameters[xParameter.ParameterName] = xParameter.Value;
            }

            _xConnection.ExecuteNonQueryCount++;
            return 1;
        }

        public override object? ExecuteScalar()
        {
            throw new NotSupportedException();
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new FakeDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            _xConnection.LastCommandText = CommandText;
            foreach (FakeDbParameter xParameter in _xParameters)
            {
                _xConnection.Parameters[xParameter.ParameterName] = xParameter.Value;
            }

            return new FakeDbDataReader([], []);
        }
    }
}
