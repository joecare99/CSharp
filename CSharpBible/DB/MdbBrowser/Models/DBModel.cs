using System;
using System.Data.OleDb;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using MdbBrowser.Models.Interfaces;
using System.Linq;

namespace MdbBrowser.Models
{
    public class DBModel : IDBModel
    {
        OleDbConnectionStringBuilder connectionStringBuilder = new();
        
        public DBModel(string filename) : this(new OleDbConnectionStringBuilder() { 
            Provider = "Microsoft.ACE.OLEDB.16.0",
           // Provider = "Microsoft.ACE.OLEDB.12.0",
           // Provider = "Microsoft.Jet.OLEDB.4.0",
            DataSource = filename, PersistSecurityInfo = false })
        {}
        public DBModel(DbConnectionStringBuilder connect)
        {
            database.ConnectionString = connect.ConnectionString;
            database.InfoMessage += (sender, e) => System.Diagnostics.Debug.WriteLine($"{sender}: {e.Message}");
            database.StateChange += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"{sender}: {e.OriginalState}=>{e.CurrentState}");

                QueryDB(sender as OleDbConnection);
            };
            if (File.Exists(connect["Data Source"].ToString()))
                database.Open();
            else
                try { 
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
                var schema = oleDbConnection.GetSchema();
                foreach (DataRow schemaDef in schema.Rows)
                {
                    dbMetaData.Add(new DBMetaData(schemaDef[0].ToString(), EKind.Schema, schemaDef, null));
                }
                var tables = oleDbConnection.GetSchema("Tables");
                foreach (DataRow table in tables.Rows)
                {
                    if (!table["TABLE_NAME"].ToString().StartsWith("MSys"))
                    {
                        dbMetaData.Add(new DBMetaData(table["TABLE_NAME"].ToString(), EKind.Table, table, null));
                    }
                }
                var views = oleDbConnection.GetSchema("Views");
                foreach (DataRow view in views.Rows)
                {
                    dbMetaData.Add(new DBMetaData(view[0].ToString(), EKind.Query, view, null ));
                }
                dbDataTypes.Clear();
                var datatypes = oleDbConnection.GetSchema("DataTypes");
                foreach (DataRow datatype in datatypes.Rows)
                {
                    dbDataTypes.Add(new DBMetaData(datatype[0].ToString(), EKind.DataTypes, datatype, new[] { datatype[5].ToString(), datatype[1].ToString(), datatype[2].ToString() }));
                }

            }
        }

        public IList<DBMetaData> QueryTable(string sTableName)
        {
            var oleDbConnection = database;
            var result = new List<DBMetaData>();
            if (oleDbConnection != null 
                && oleDbConnection.State == ConnectionState.Open)
            {
                var schema = oleDbConnection.GetSchema("Columns", new string[] { null, null, sTableName });
                foreach (DataRow schemaDef in schema.Rows)
                {
                    result.Add(new DBMetaData(schemaDef[3].ToString(), EKind.Column, schemaDef, new[] { GetTypeName((int)schemaDef[11]), schemaDef[13].ToString() } ));
                }
            }
            return result;
        }

        private string GetTypeName(int iID) {
            foreach (var dt in dbDataTypes)
                if (dt.Data?.ToList()[1] is string sID && int.TryParse(sID, out int iID2) && iID2 == iID)
                    return dt.Name;
            
            return iID switch { 
                130 => "VarChar",
                _ => "Unknown"
            };
        }

        public DataTable? QueryTableData(string value)
        {
            var result = new DataTable();
            result.Load(new OleDbCommand($"SELECT * FROM {value}",database).ExecuteReader());
            return result;
        }

        public OleDbConnection database { get; } = new();

        public List<DBMetaData> dbMetaData = new();
        public List<DBMetaData> dbDataTypes = new();
    }
}
