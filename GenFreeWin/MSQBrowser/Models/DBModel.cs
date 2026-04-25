using System;
using System.Data.OleDb;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using MSQBrowser.Models.Interfaces;
using System.Linq;
using BaseLib.Helper;
using MySqlConnector;
using System.Security;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSQBrowser.Models
{
    public class DBModel : IDBModel
    {
        // Internal Factory
        Func<DbConnection> GetConnection = () => new MySqlConnection();
        Func<string, DbConnection, DbCommand> GetCommand = (s, c) => new MySqlCommand(s, c as MySqlConnection);
        Func<DbCommand, DbDataAdapter> GetDataAdapter = (c) => new MySqlDataAdapter(c as MySqlCommand);
        Func<DbCommandBuilder> GetCommandBuilder = () => new MySqlCommandBuilder();

        public DBModel(string sURL, string sUser, SecureString sPasswd, string sDB) : this(
           new NetworkCredential(sUser, sPasswd) is NetworkCredential nwc ? new MySqlConnectionStringBuilder()
           { Server = sURL, UserID = nwc.UserName, Password = nwc.Password, Database = sDB, CharacterSet = "UTF8", ConvertZeroDateTime = true } : null!)
        { }
        public DBModel(MySqlConnectionStringBuilder connect)
        {
            connect = new MySqlConnectionStringBuilder(connect.ConnectionString)
            {
                ConvertZeroDateTime = true
            };

            database ??= GetConnection();
            database.ConnectionString = connect.ConnectionString;
            DbName = connect.Database;
            //    database.InfoMessage += (sender, e) => System.Diagnostics.Debug.WriteLine($"{sender}: {e.Message}");
            database.StateChange += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"{sender}: {e.OriginalState}=>{e.CurrentState}");

                QueryDB(sender as DbConnection);
            };
            System.Diagnostics.Debug.WriteLine($"DBModel: {Directory.GetCurrentDirectory()}");
            try
            {
                database.Open();
            }
            catch
            {
                try
                {
                    // copy empty database to sURL
                    database.OpenAsync();
                }
                catch (Exception) { }
            }
        }

        private void QueryDB(DbConnection dbConnection)
        {
            if (dbConnection != null && dbConnection.State == ConnectionState.Open)
            {
                dbMetaData.Clear();
                foreach (var (schema, field, kind, restr) in new (string, string, EKind, string?[])[] {
                    ("MetaDataCollections","CollectionName",EKind.Schema,Array.Empty<string?>()),
                    ("Tables","TABLE_NAME",EKind.Table, new[]{null,DbName }),
                    ("Views","TABLE_NAME",EKind.Query, []),
                    ("Columns","COLUMN_NAME",EKind.Column, new[]{null,DbName }),
                    ("Indexes","INDEX_NAME",EKind.Index, new[]{null,DbName })
                })
                {

                    DataTable schemaDat;

                    schemaDat = dbConnection.GetSchema(schema, restr);

                    foreach (DataRow schemaDef in schemaDat.Rows)
                    {
                        if (kind != EKind.Table || !schemaDef[field].ToString().StartsWith("MSys"))

                            dbMetaData.Add(new DBMetaData(schemaDef[field].ToString(), kind, schemaDef, null));
                    }
                }
                dbDataTypes.Clear();
                var datatypes = dbConnection.GetSchema("DataTypes");
                foreach (DataRow datatype in datatypes.Rows)
                {
                    dbDataTypes.Add(new DBMetaData(datatype[0].ToString(), EKind.DataTypes, datatype, new[] { datatype[5].ToString(), datatype[1].ToString(), datatype[2].ToString() }));
                }

            }
        }

        public IList<DBMetaData> QueryTable(string sTableName)
        {
            var dbConnection = database;
            var result = new List<DBMetaData>();
            if (sTableName.IsValidIdentifyer()
                && dbConnection != null
                && dbConnection.State == ConnectionState.Open)
            {
                var schema = dbConnection.GetSchema(OleDbMetaDataCollectionNames.Columns, new string?[] { null, DbName, sTableName, null });
                foreach (DataRow schemaDef in schema.Rows)
                    try
                    {
                        result.Add(new DBMetaData(schemaDef[3].ToString(), EKind.Column, schemaDef, new[] { schemaDef[7].ToString(), schemaDef[14].ToString() }));
                    }
                    catch { }
                //         schema = dbConnection.GetSchema(OleDbMetaDataCollectionNames.Indexes, new string?[] { null, DbName, sTableName,null,null });

            }
            return result;
        }

        public string GetTypeName(int iID)
        {
            foreach (var dt in dbDataTypes)
                if (dt.Data.ToList()[1] == iID.ToString())
                    return dt.Name;

            return iID switch
            {
                130 => "VarChar",
                _ => "Unknown"
            };
        }

        public DataTable? QueryTableData(string value)
        {
            var result = new DataTable();
            if (value.IsValidIdentifyer())
                try
                {
                    var da = GetDataAdapter(GetCommand($"SELECT * FROM {value}", database));
                    var bldr = GetCommandBuilder();
                    bldr.DataAdapter = da;
                    da.InsertCommand = bldr.GetInsertCommand();
                    da.UpdateCommand = bldr.GetUpdateCommand();
                    da.DeleteCommand = bldr.GetDeleteCommand();
                    da.Fill(result);
                    result.RowChanged += (s, e) => TableChanged(s, e, da);
                    //result.TableNewRow += (sender, e) => System.Diagnostics.Debug.WriteLine($"New({sender}):{e.Row}");  
                }
                catch { }
            return result;
        }

        public async Task<TablePageResult> QueryTableDataPageAsync(string value, IEnumerable<DBMetaData> columns, int offset, int pageSize, CancellationToken cancellationToken = default)
        {
            var result = new DataTable();
            if (!value.IsValidIdentifyer()
                || database is not MySqlConnection connection
                || connection.State != ConnectionState.Open)
            {
                return new TablePageResult { Data = result, Offset = Math.Max(0, offset), PageSize = Math.Max(1, pageSize) };
            }

            var effectiveOffset = Math.Max(0, offset);
            var effectivePageSize = Math.Max(1, pageSize);
            var sql = $"SELECT {BuildSelectClause(columns)} FROM {EscapeIdentifier(value)} LIMIT @limit OFFSET @offset";

            await using var pageConnection = new MySqlConnection(connection.ConnectionString);
            await pageConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using var command = new MySqlCommand(sql, pageConnection);
            command.Parameters.AddWithValue("@limit", effectivePageSize + 1);
            command.Parameters.AddWithValue("@offset", effectiveOffset);

            using var adapter = new MySqlDataAdapter(command);
            await Task.Run(() => adapter.Fill(result), cancellationToken).ConfigureAwait(false);

            var hasMoreRows = result.Rows.Count > effectivePageSize;
            if (hasMoreRows)
            {
                result.Rows[result.Rows.Count - 1].Delete();
                result.AcceptChanges();
            }

            NormalizeZeroDateTimeValues(result, columns);

            return new TablePageResult
            {
                Data = result,
                Offset = effectiveOffset,
                PageSize = effectivePageSize,
                HasMoreRows = hasMoreRows
            };
        }

        private void TableChanged(object sender, DataRowChangeEventArgs e, DbDataAdapter da)
        {
            if (sender is DataTable dt && e.Action != DataRowAction.Commit)
            {
                System.Diagnostics.Debug.WriteLine($"Changed({sender}):{e.Action}:{e.Row}");
                da.Update(dt);
                dt.AcceptChanges();
            }
            else if (e.Action == DataRowAction.Commit)
            {
                System.Diagnostics.Debug.WriteLine($"Changed({sender}):{e.Action}:{e.Row.ItemArray[0]}");
                //       da.Update(new[] { e.Row });
            }
        }

        public DataTable? QuerySchema(string value)
        {
            return value.IsValidIdentifyer() ? database.GetSchema(value) : null;
        }

        private static string BuildSelectClause(IEnumerable<DBMetaData> columns)
        {
            var mappedColumns = columns?.Select(BuildColumnSelectExpression)
                .Where(expression => !string.IsNullOrWhiteSpace(expression))
                .ToArray();

            return mappedColumns is { Length: > 0 } ? string.Join(", ", mappedColumns) : "*";
        }

        private static string BuildColumnSelectExpression(DBMetaData column)
        {
            var columnName = EscapeIdentifier(column.Name);
            var dataType = GetSchemaValue(column, "DATA_TYPE")?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(dataType))
            {
                return columnName;
            }

            if (IsBinaryType(dataType))
            {
                return $"CASE WHEN {columnName} IS NULL THEN NULL ELSE CONCAT('<', OCTET_LENGTH({columnName}), ' bytes>') END AS {columnName}";
            }

            if (IsLongTextType(dataType))
            {
                return $"CASE WHEN {columnName} IS NULL THEN NULL WHEN CHAR_LENGTH({columnName}) > 256 THEN CONCAT(LEFT({columnName}, 256), '…') ELSE {columnName} END AS {columnName}";
            }

            return columnName;
        }

        private static bool IsBinaryType(string dataType)
        {
            switch (dataType)
            {
                case "binary":
                case "varbinary":
                case "blob":
                case "tinyblob":
                case "mediumblob":
                case "longblob":
                case "geometry":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsLongTextType(string dataType)
        {
            switch (dataType)
            {
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                case "json":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsDateTimeType(string dataType)
        {
            switch (dataType)
            {
                case "date":
                case "datetime":
                case "timestamp":
                case "time":
                    return true;
                default:
                    return false;
            }
        }

        private static void NormalizeZeroDateTimeValues(DataTable table, IEnumerable<DBMetaData> columns)
        {
            if (table.Rows.Count == 0)
            {
                return;
            }

            var dateTimeColumns = columns
                .Where(column => IsDateTimeType(GetSchemaValue(column, "DATA_TYPE")?.ToLowerInvariant() ?? string.Empty))
                .Select(column => column.Name)
                .Where(table.Columns.Contains)
                .ToArray();

            foreach (DataRow row in table.Rows)
            {
                foreach (var columnName in dateTimeColumns)
                {
                    if (row[columnName] is DateTime dateTimeValue
                        && dateTimeValue == DateTime.MinValue)
                    {
                        row[columnName] = DBNull.Value;
                    }
                }
            }
        }

        private static string? GetSchemaValue(DBMetaData column, string fieldName)
        {
            if (column.This is not DataRow row || !row.Table.Columns.Contains(fieldName))
            {
                return null;
            }

            return row[fieldName]?.ToString();
        }

        private static string EscapeIdentifier(string identifier)
        {
            var builder = new StringBuilder(identifier.Length + 2);
            builder.Append('`');
            builder.Append(identifier.Replace("`", "``"));
            builder.Append('`');
            return builder.ToString();
        }

        public DbConnection? database { get; } = null;
        public string DbName { get; }

        public List<DBMetaData> dbMetaData = new();
        public List<DBMetaData> dbDataTypes = new();
    }
}
