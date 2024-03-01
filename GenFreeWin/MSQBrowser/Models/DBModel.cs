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

namespace MSQBrowser.Models
{
    public class DBModel : IDBModel
    {
        // Internal Factory
        Func<DbConnection> GetConnection = () => new MySqlConnection();
        Func<string,DbConnection,DbCommand> GetCommand = (s,c) => new MySqlCommand(s,c as MySqlConnection);
        Func<DbCommand,DbDataAdapter> GetDataAdapter = (c) => new MySqlDataAdapter(c as MySqlCommand);
        Func<DbCommandBuilder> GetCommandBuilder = () => new MySqlCommandBuilder();

        public DBModel(string sURL,string sUser, SecureString sPasswd, string sDB) : this(
           new NetworkCredential(sUser,sPasswd ) is NetworkCredential nwc? new MySqlConnectionStringBuilder() 
        { Server = sURL, UserID = nwc.UserName, Password = nwc.Password,Database=sDB, CharacterSet = "UTF8" }:null!)
        { }
        public DBModel(MySqlConnectionStringBuilder connect)
        {
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
                foreach (var (schema, field, kind,restr) in new (string, string, EKind, string?[])[] {
                    ("MetaDataCollections","CollectionName",EKind.Schema,Array.Empty<string?>()),
                    ("Tables","TABLE_NAME",EKind.Table, new[]{null,DbName }),
                    ("Views","TABLE_NAME",EKind.Query, new[]{null,DbName }),
                    ("Columns","COLUMN_NAME",EKind.Column, new[]{null,DbName }),
                    ("Indexes","INDEX_NAME",EKind.Index, new[]{null,DbName })
                })
                {
                    var schemaDat = dbConnection.GetSchema(schema,restr);
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
                    result.RowChanged += (s,e)=>TableChanged(s,e,da);
                    //result.TableNewRow += (sender, e) => System.Diagnostics.Debug.WriteLine($"New({sender}):{e.Row}");  
                }
                catch { }
            return result;
        }

        private void TableChanged(object sender, DataRowChangeEventArgs e, DbDataAdapter da)
        {
            if (sender is DataTable dt && e.Action != DataRowAction.Commit)
            {
                System.Diagnostics.Debug.WriteLine($"Changed({sender}):{e.Action}:{e.Row}");
                da.Update(dt);
                dt.AcceptChanges();
            }
            else if(e.Action == DataRowAction.Commit)
            {
                System.Diagnostics.Debug.WriteLine($"Changed({sender}):{e.Action}:{e.Row.ItemArray[0]}");
                //       da.Update(new[] { e.Row });
            }
        }

        public DataTable? QuerySchema(string value)
        {
            return value.IsValidIdentifyer() ? database.GetSchema(value) : null;
        }
        public DbConnection? database { get; } = null;
        public string DbName { get; }

        public List<DBMetaData> dbMetaData = new();
        public List<DBMetaData> dbDataTypes = new();
    }
}
