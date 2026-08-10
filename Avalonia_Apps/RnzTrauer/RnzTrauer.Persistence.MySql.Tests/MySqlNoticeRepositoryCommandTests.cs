using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Db.Core.Abstractions.Sql.Interfaaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Places;
using RnzTrauer.Persistence.MySql;

#pragma warning disable CS8764

namespace RnzTrauer.Persistence.MySql.Tests;

[TestClass]
public sealed class MySqlNoticeRepositoryCommandTests
{
    [TestMethod]
    public async Task SaveAsync_CapturesParameterizedCommandAndUsesSingleStatementBoundary()
    {
        var connection = new RecordingConnection();
        var repository = CreateRepository(connection);
        var notice = new DeathNotice
        {
            Id = 17,
            FamilyName = "Müller",
            GivenName = "Anna",
            BirthQualification = DateQualification.Estimated,
        };

        await repository.SaveAsync(notice);

        Assert.IsNotNull(connection.LastCommand);
        StringAssert.Contains(connection.LastCommand!.CommandText, "UPDATE `Anzeigen`");
        Assert.AreEqual("Müller", connection.LastCommand.GetValue("@family"));
        Assert.AreEqual("est.", connection.LastCommand.GetValue("@birthModif"));
        Assert.AreEqual(17L, connection.LastCommand.GetValue("@id"));
        Assert.AreEqual(1, connection.ExecuteNonQueryCount);
        Assert.AreEqual(0, connection.BeginTransactionCount);
        Assert.AreEqual(1, connection.OpenCount);
        Assert.AreEqual(1, connection.CloseCount);
    }

    [TestMethod]
    public async Task UpsertImportedAsync_BindsBusinessKeyAndReturnsSuccessfulExecution()
    {
        var connection = new RecordingConnection { AffectedRows = 2 };
        var repository = CreateRepository(connection);
        var notice = new DeathNotice
        {
            OrderNumber = "A-42",
            Path = "/archive",
            PdfFile = "notice.pdf",
            PngFile = "notice.png",
        };

        var result = await repository.UpsertImportedAsync(notice);

        Assert.IsTrue(result);
        Assert.IsNotNull(connection.LastCommand);
        StringAssert.Contains(connection.LastCommand!.CommandText, "ON DUPLICATE KEY UPDATE");
        Assert.AreEqual("A-42", connection.LastCommand.GetValue("@order"));
        Assert.AreEqual(2, connection.AffectedRows);
    }

    [TestMethod]
    public async Task UpsertImportedAsync_ReturnsTrueWhenProviderReportsNoAffectedRows()
    {
        var connection = new RecordingConnection { AffectedRows = 0 };
        var repository = CreateRepository(connection);

        var result = await repository.UpsertImportedAsync(new DeathNotice
        {
            OrderNumber = "A-43",
        });

        Assert.IsTrue(result);
        Assert.AreEqual(1, connection.ExecuteNonQueryCount);
        Assert.AreEqual(0, connection.BeginTransactionCount);
    }

    [TestMethod]
    public async Task PlaceCoordinateStore_GetAsync_MapsOptionalLegacyColumns()
    {
        var table = new DataTable();
        table.Columns.Add("Ortname", typeof(string));
        table.Columns.Add("Latitude", typeof(double));
        table.Columns.Add("Longitude", typeof(double));
        table.Rows.Add("Heidelberg", 49.3988, 8.6724);
        var connection = new RecordingConnection { Reader = table.CreateDataReader() };

        var result = await CreatePlaceStore(connection).GetAsync(" heidelberg ");

        Assert.IsNotNull(result);
        Assert.AreEqual("Heidelberg", result!.Place);
        Assert.AreEqual(49.3988, result.Latitude);
        Assert.AreEqual(8.6724, result.Longitude);
        Assert.AreEqual(1, connection.OpenCount);
        Assert.AreEqual(1, connection.CloseCount);
    }

    [TestMethod]
    public async Task PlaceCoordinateStore_SaveAsync_BindsNormalizedCoordinates()
    {
        var connection = new RecordingConnection();

        await CreatePlaceStore(connection).SaveAsync(
            new PlaceCoordinate(" Heidelberg ", 49.3988, 8.6724, "fixture", false));

        Assert.IsNotNull(connection.LastCommand);
        StringAssert.Contains(connection.LastCommand!.CommandText, "UPDATE `Orte`");
        Assert.AreEqual("Heidelberg", connection.LastCommand.GetValue("@place"));
        Assert.AreEqual(49.3988, connection.LastCommand.GetValue("@latitude"));
        Assert.AreEqual(8.6724, connection.LastCommand.GetValue("@longitude"));
        Assert.AreEqual(1, connection.ExecuteNonQueryCount);
    }

    [TestMethod]
    public async Task PlaceCoordinateStore_ProbeAsync_ClassifiesUnknownColumn()
    {
        var connection = new RecordingConnection
        {
            ReaderException = new SyntheticDbException(1054),
        };

        var report = await CreatePlaceStore(connection).ProbeAsync();

        Assert.AreEqual(CoordinateSchemaStatus.Missing, report.Status);
        Assert.IsFalse(report.CanPersist);
    }

    [TestMethod]
    public async Task FindAsync_MapsNullableDatesQualificationsCategoryAndOptionalFields()
    {
        var table = new DataTable();
        table.Columns.Add("idAnzeige", typeof(long));
        table.Columns.Add("Auftrag", typeof(string));
        table.Columns.Add("Stichwort", typeof(string));
        table.Columns.Add("Nachname", typeof(string));
        table.Columns.Add("Vorname", typeof(string));
        table.Columns.Add("Geburtsname", typeof(string));
        table.Columns.Add("Titel", typeof(string));
        table.Columns.Add("Geschlecht", typeof(string));
        table.Columns.Add("Erscheinungsdatum", typeof(DateTime));
        table.Columns.Add("Geb", typeof(DateTime));
        table.Columns.Add("GebModif", typeof(string));
        table.Columns.Add("Gest", typeof(DateTime));
        table.Columns.Add("GestModif", typeof(string));
        table.Columns.Add("Begr", typeof(DateTime));
        table.Columns.Add("Ort", typeof(string));
        table.Columns.Add("Rubrik", typeof(long));
        table.Columns.Add("Text", typeof(string));
        table.Columns.Add("Pfad", typeof(string));
        table.Columns.Add("LinkID", typeof(long));
        table.Columns.Add("ProfImgCount", typeof(long));
        table.Columns.Add("PDF", typeof(string));
        table.Columns.Add("PNG", typeof(string));
        table.Columns.Add("ProfileImg", typeof(string));
        table.Columns.Add("TimeStamp", typeof(DateTime));
        table.Rows.Add(91L, "A-91", DBNull.Value, "Müller", "Anna", DBNull.Value,
            DBNull.Value, "F", DBNull.Value, new DateTime(1940, 2, 3), "est.",
            DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
            DBNull.Value, "/archive", DBNull.Value, 2L, "a.pdf", DBNull.Value,
            DBNull.Value, DBNull.Value);

        var connection = new RecordingConnection { Reader = table.CreateDataReader() };
        var notices = await CreateRepository(connection).FindAsync(new NoticeFilter());

        var notice = AssertSingle(notices);
        Assert.AreEqual(91L, notice.Id);
        Assert.AreEqual("A-91", notice.OrderNumber);
        Assert.AreEqual(new DateTime(1940, 2, 3), notice.BirthDate);
        Assert.AreEqual(DateQualification.Estimated, notice.BirthQualification);
        Assert.AreEqual(DateQualification.Exact, notice.DeathQualification);
        Assert.AreEqual(AdvertisementCategory.DeathNotice, notice.Category);
        Assert.IsNull(notice.LinkedNoticeId);
        Assert.AreEqual(2, notice.ProfileImageCount);
        Assert.AreEqual("a.pdf", notice.PdfFile);
        Assert.IsNull(notice.PngFile);
        Assert.IsNull(notice.ProfileImage);
    }

    [TestMethod]
    public async Task FindAsync_DisposesReaderAndConnection()
    {
        var table = new DataTable();
        table.Columns.Add("idAnzeige", typeof(long));
        table.Columns.Add("Auftrag", typeof(string));
        using var reader = table.CreateDataReader();
        var connection = new RecordingConnection { Reader = reader };

        await CreateRepository(connection).FindAsync(new NoticeFilter());

        Assert.IsTrue(reader.IsClosed);
        Assert.AreEqual(1, connection.CloseCount);
    }

    [TestMethod]
    public async Task FindAsync_PropagatesProviderReaderFailureAndDisposesConnection()
    {
        var failure = new InvalidOperationException("reader failed");
        var connection = new RecordingConnection { ReaderException = failure };

        InvalidOperationException? thrown = null;
        try
        {
            await CreateRepository(connection).FindAsync(new NoticeFilter());
        }
        catch (InvalidOperationException exception)
        {
            thrown = exception;
        }

        Assert.IsNotNull(thrown);
        Assert.AreSame(failure, thrown);
        Assert.AreEqual(1, connection.CloseCount);
    }

    private static DeathNotice AssertSingle(IReadOnlyList<DeathNotice> notices)
    {
        Assert.AreEqual(1, notices.Count);
        return notices[0];
    }

    private static MySqlNoticeRepository CreateRepository(RecordingConnection connection)
    {
        return new MySqlNoticeRepository(
            new RecordingFactory(connection),
            new DictionarySettings());
    }

    private static MySqlPlaceCoordinateStore CreatePlaceStore(RecordingConnection connection)
    {
        return new MySqlPlaceCoordinateStore(
            new RecordingFactory(connection),
            new DictionarySettings());
    }

    private sealed class RecordingFactory : IDbConnectionFactory
    {
        private readonly RecordingConnection _connection;

        public RecordingFactory(RecordingConnection connection)
        {
            _connection = connection;
        }

        public IDbConnection CreateConnection(IDBSettings xSettings) => _connection;

        public IDbStatementRenderer CreateStatementRenderer(IDbConnection dBConnection)
        {
            throw new NotSupportedException();
        }

        public IDBSettings CreateSettingsStub() => new DictionarySettings();
    }

    private sealed class DictionarySettings : Dictionary<string, object>, IDBSettings;

    private sealed class SyntheticDbException : DbException
    {
        private readonly int _errorCode;

        public SyntheticDbException(int errorCode) => _errorCode = errorCode;

        public override int ErrorCode => _errorCode;
    }

    private sealed class RecordingConnection : DbConnection
    {
        private ConnectionState _state = ConnectionState.Closed;

        public FakeDbCommand? LastCommand { get; private set; }
        public int AffectedRows { get; set; } = 1;
        public int ExecuteNonQueryCount { get; private set; }
        public int BeginTransactionCount { get; private set; }
        public int OpenCount { get; private set; }
        public int CloseCount { get; private set; }
        public DbDataReader? Reader { get; set; }
        public Exception? ReaderException { get; set; }

        public override string? ConnectionString { get; set; } = string.Empty;
        public override string Database => "RNZ";
        public override string DataSource => "test";
        public override string ServerVersion => "test";
        public override ConnectionState State => _state;

        public object? GetValue(string parameterName) => LastCommand?.GetValue(parameterName);

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
            CloseCount++;
            _state = ConnectionState.Closed;
        }

        public override void Open()
        {
            OpenCount++;
            _state = ConnectionState.Open;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _state != ConnectionState.Closed)
                Close();
            base.Dispose(disposing);
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            BeginTransactionCount++;
            return new RecordingTransaction(this, isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            LastCommand = new FakeDbCommand(this);
            return LastCommand;
        }

        public void RecordExecution()
        {
            ExecuteNonQueryCount++;
        }
    }

    private sealed class RecordingTransaction : DbTransaction
    {
        private readonly DbConnection _connection;
        private readonly IsolationLevel _isolationLevel;

        public RecordingTransaction(DbConnection connection, IsolationLevel isolationLevel)
        {
            _connection = connection;
            _isolationLevel = isolationLevel;
        }

        public override IsolationLevel IsolationLevel => _isolationLevel;
        protected override DbConnection DbConnection => _connection;
        public override void Commit() { }
        public override void Rollback() { }
    }

    private sealed class FakeDbCommand : DbCommand
    {
        private readonly RecordingConnection _connection;
        private readonly FakeParameterCollection _parameters = new();

        public FakeDbCommand(RecordingConnection connection)
        {
            _connection = connection;
        }

        public override string? CommandText { get; set; } = string.Empty;
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection? DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => _parameters;
        protected override DbTransaction? DbTransaction { get; set; }

        public object? GetValue(string parameterName)
        {
            return _parameters.Cast<DbParameter>()
                .Single(parameter => parameter.ParameterName == parameterName)
                .Value;
        }

        public override void Cancel() { }
        public override int ExecuteNonQuery()
        {
            _connection.RecordExecution();
            return _connection.AffectedRows;
        }

        public override object? ExecuteScalar() => null;
        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => new FakeDbParameter();
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            => _connection.ReaderException is not null
                ? throw _connection.ReaderException
                : _connection.Reader ?? throw new InvalidOperationException("No reader configured.");
    }

    private sealed class FakeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public override bool IsNullable { get; set; }
        public override string? ParameterName { get; set; } = string.Empty;
        public override string? SourceColumn { get; set; } = string.Empty;
        public override bool SourceColumnNullMapping { get; set; }
        public override object? Value { get; set; }
        public override int Size { get; set; }
        public override byte Precision { get; set; }
        public override byte Scale { get; set; }
        public override void ResetDbType() { }
    }

    private sealed class FakeParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _items = [];

        public override int Count => _items.Count;
        public override object SyncRoot => ((ICollection)_items).SyncRoot;
        public override int Add(object value)
        {
            _items.Add((DbParameter)value);
            return _items.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
                Add(value!);
        }

        public override void Clear() => _items.Clear();
        public override bool Contains(object value) => _items.Contains((DbParameter)value);
        public override bool Contains(string value) => _items.Any(parameter => parameter.ParameterName == value);
        public override void CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);
        public override IEnumerator GetEnumerator() => _items.GetEnumerator();
        public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => _items.FindIndex(parameter => parameter.ParameterName == parameterName);
        public override void Insert(int index, object value) => _items.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _items.Remove((DbParameter)value);
        public override void RemoveAt(int index) => _items.RemoveAt(index);
        public override void RemoveAt(string parameterName) => RemoveAt(IndexOf(parameterName));
        protected override DbParameter GetParameter(int index) => _items[index];
        protected override DbParameter GetParameter(string parameterName)
            => _items.Single(parameter => parameter.ParameterName == parameterName);
        public override int GetHashCode() => base.GetHashCode();
        protected override void SetParameter(int index, DbParameter value) => _items[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index < 0)
                Add(value);
            else
                _items[index] = value;
        }
    }

    #pragma warning restore CS8764
}
