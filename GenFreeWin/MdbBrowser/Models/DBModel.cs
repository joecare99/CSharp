using System;
using ADOX;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MdbBrowser.Models
{
    public class DBModel
    {

        public DBModel(string filename)
        {
            string connectionString = string.Format("Provider={0}; Data Source={1}; Jet OLEDB:Engine Type={2}",
    "Microsoft.Jet.OLEDB.4.0",
    filename,
    5);


            catalog = new Catalog();
            if (File.Exists(filename))
                catalog.ActiveConnection = connectionString;
            else
                try { catalog.Create(connectionString); }
                catch (Exception) { }
            if (catalog.ActiveConnection != null)
            {
                dbMetaData.Clear();
                foreach (Table tableDef in catalog.Tables)
                {
                    dbMetaData.Add(new DBMetaData(tableDef.Name, EKind.Table, tableDef, tableDef.Columns.Cast<Column>().Select(f => f.Name)));
                }
                foreach (View view in catalog.Views)
                {
                    dbMetaData.Add(new DBMetaData(view.Name, EKind.Query,view,new string[]{ view.Command.ToString() }));
                }
                try
                {
                    foreach (Procedure procedure in catalog.Procedures)
                    {
                        dbMetaData.Add(new DBMetaData(procedure.Name, EKind.Macro,procedure,new string[] { procedure.Command.ToString() }));
                    }
                }
                catch (Exception) { }
                try
                {
                    foreach (Group group in catalog.Groups)
                    {
                        dbMetaData.Add(new DBMetaData(group.Name, EKind.Group,group, null));
                    }
                }
                catch (Exception) { }
                try
                {
                    foreach (User user in catalog.Users)
                    {
                        dbMetaData.Add(new DBMetaData(user.Name, EKind.User,user, null));
                    }
                }
                catch (Exception) { }

            }
        }

        public Catalog catalog { get; }

        public List<DBMetaData> dbMetaData = new();
    }
}
