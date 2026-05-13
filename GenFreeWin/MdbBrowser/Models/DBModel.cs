using System;
using System.Data.OleDb;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using MdbBrowser.Models.Interfaces;
using System.Linq;
using BaseLib.Helper;
using Db.Core.Abstractions.Sql;
using GenFree.Data.DB;
using Db.Core.Abstractions.Sql.Interfaaces;

namespace MdbBrowser.Models
{
    public class DBModel : IDBModel
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IDbStatementRenderer _statementRenderer;

        public DBModel(string filename)
        {
            _connectionFactory = new OleDbConnectionFactory();
            var _dbSettings = _connectionFactory.CreateSettingsStub();
            _dbSettings[nameof(OleDbConnectionStringBuilder.DataSource)] = filename;
            database = (OleDbConnection)_connectionFactory.CreateConnection(_dbSettings);
            _statementRenderer = _connectionFactory.CreateStatementRenderer(database);
            //    database.InfoMessage += (sender, e) => System.Diagnostics.Debug.WriteLine($"{sender}: {e.Message}");
            database.StateChange += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"{sender}: {e.OriginalState}=>{e.CurrentState}");

                QueryDB(sender as OleDbConnection);
            };
            System.Diagnostics.Debug.WriteLine($"DBModel: {Directory.GetCurrentDirectory()}");
            if (File.Exists(_dbSettings[nameof(OleDbConnectionStringBuilder.DataSource)].ToString()))
                database.Open();
            else
                try
                {
                    // copy empty database to filename
                    database.OpenAsync();
                }
                catch (Exception) { }
        }

        private void QueryDB(OleDbConnection oleDbConnection)
        {
            if (oleDbConnection != null && oleDbConnection.State == ConnectionState.Open)
            {
                dbMetaData.Clear();
                foreach (var (schema, field, kind) in new (string, string[], EKind)[] {
                    ("MetaDataCollections",["CollectionName"],EKind.Schema),
                    ("Tables",["TABLE_NAME"],EKind.Table),
                    ("Views",["TABLE_NAME"],EKind.Query),
                    ("Columns",["TABLE_NAME","COLUMN_NAME"],EKind.Column),
                    ("Indexes",["TABLE_NAME","INDEX_NAME"],EKind.Index)
                })
                    try
                    {
                        var schemaDat = oleDbConnection.GetSchema(schema);
                        foreach (DataRow schemaDef in schemaDat.Rows)
                        {
                            if (kind != EKind.Table || !schemaDef[field.First()].ToString().StartsWith("MSys"))

                                dbMetaData.Add(new DBMetaData(string.Join(".", field.Select(f => schemaDef[f].ToString())), kind, schemaDef, null));
                        }
                    }
                    catch { }
                dbDataTypes.Clear();
                var datatypes = oleDbConnection.GetSchema("DataTypes");
                foreach (DataRow datatype in datatypes.Rows)
                {
                    dbDataTypes.Add(new DBMetaData(datatype[0].ToString(), EKind.DataTypes, datatype, 
                        new[] { datatype[5].ToString(), datatype[1].ToString(), datatype[2].ToString() }));
                }

            }
        }

        public IList<DBMetaData> QueryTable(string sTableName)
        {
            var oleDbConnection = database;
            var result = new List<DBMetaData>();
            if (sTableName.IsValidIdentifyer()
                && oleDbConnection != null
                && oleDbConnection.State == ConnectionState.Open)
            {
                var schema = oleDbConnection.GetSchema(OleDbMetaDataCollectionNames.Columns, new string?[] { null, null, sTableName, null });
                foreach (DataRow schemaDef in schema.Rows)
                {
                    result.Add(new DBMetaData(schemaDef[3].ToString(), EKind.Column, schemaDef, new[] { GetTypeName((int)schemaDef[11]), schemaDef[13].ToString() }));
                }
                schema = oleDbConnection.GetSchema(OleDbMetaDataCollectionNames.Indexes, new string?[] { null, null, sTableName,null,null });

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
                    using var command = _statementRenderer.CreateQuery(new DbSelectStatement(value));
                    result.Load(command.ExecuteReader());
                }
                catch { }
            return result;
        }

        public DataTable? QuerySchema(string value)
        {
            return value.IsValidIdentifyer() ? database.GetSchema(value) : null;
        }
        public OleDbConnection database { get; }

        public List<DBMetaData> dbMetaData = new();
        public List<DBMetaData> dbDataTypes = new();
    }
}
