using System;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Data;

namespace MdbBrowser.Models
{
    public class DBModel
    {
        OleDbConnectionStringBuilder connectionStringBuilder = new();
        
        public DBModel(string filename) : this(new OleDbConnectionStringBuilder() { Provider = "Microsoft.ACE.OLEDB.16.0", DataSource = filename, PersistSecurityInfo = false })
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
                    dbMetaData.Add(new DBMetaData(table["TABLE_NAME"].ToString(), EKind.Table, table, null));
                }
                var views = oleDbConnection.GetSchema("Views");
                foreach (DataRow view in views.Rows)
                {
                    dbMetaData.Add(new DBMetaData(view[0].ToString(), EKind.Query, view, null ));
                }
            }
        }
        public OleDbConnection database { get; } = new();

        public List<DBMetaData> dbMetaData = new();
    }
}
