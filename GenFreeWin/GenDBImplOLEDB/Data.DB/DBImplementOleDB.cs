using BaseLib.Helper;
using GenFree.Interfaces.DB;
using GenFree.Properties;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;

namespace GenFree.Data.DB;

public static class DBImplementOleDB
{
    public class DBEngine : IDBEngine
    {
        public IList<IDBWorkSpace> Workspaces { get; } = new List<IDBWorkSpace>() { };


        public IDatabase OpenDatabase(string path, bool v2, bool v3, string v4)
        {
            OleDbConnection result;
            if (IntPtr.Size == 8)
                result = new OleDbConnection()
                {
                    ConnectionString = new OleDbConnectionStringBuilder()
                    {
                        Provider = Settings.Default.OleDB_Provider64,
                        DataSource = path,
                        PersistSecurityInfo = false
                    }.ConnectionString
                };
            else
                result = new OleDbConnection()
                {
                    ConnectionString = new OleDbConnectionStringBuilder()
                    {
                        Provider = Settings.Default.OleDB_Provider,
                        DataSource = path,
                        PersistSecurityInfo = false
                    }.ConnectionString
                };

            result.Open();
            return new CDatabase(result);
        }

        public void CompactDatabase(string v1, string v2)
        {
            // Compacting einer Access-Datenbank via JRO (Jet and Replication Objects)
            // Funktioniert nur unter Windows und .NET Framework, nicht unter .NET Core/5+/8!
            // Referenz auf COM: Microsoft Jet and Replication Objects 2.6 Library (jro.dll) erforderlich
            // Alternativ: dynamisches COM-Interop

            // Prüfe, ob Quelldatei existiert
            if (!System.IO.File.Exists(v1))
                throw new System.IO.FileNotFoundException("Quelldatei nicht gefunden.", v1);

            // Ziel ggf. löschen
            if (System.IO.File.Exists(v2))
                System.IO.File.Delete(v2);

            // COM-Typ dynamisch laden (spätes Binding, keine Projekt-Referenz nötig)
            Type? jroType = Type.GetTypeFromProgID("JRO.JetEngine");
            if (jroType == null || IntPtr.Size==8)
                throw new NotSupportedException("JRO.JetEngine COM-Komponente nicht verfügbar. Datenbank-Komprimierung nicht möglich.");

            object jetEngine = Activator.CreateInstance(jroType);

            // Connection-Strings für Quelle und Ziel
            var src = $"Provider={(IntPtr.Size==8? Settings.Default.OleDB_Provider64 :Settings.Default.OleDB_Provider)};Data Source={v1};Jet OLEDB:Engine Type=5";
            var dst = $"Provider={(IntPtr.Size==8? Settings.Default.OleDB_Provider64 :Settings.Default.OleDB_Provider)};Data Source={v2};Jet OLEDB:Engine Type=5";

            // CompactDatabase aufrufen
            jroType.InvokeMember("CompactDatabase",
                System.Reflection.BindingFlags.InvokeMethod,
                null,
                jetEngine,
                new object[] { src, dst });

            // Optional: Quelldatei durch komprimierte Datei ersetzen
            // System.IO.File.Delete(v1);
            // System.IO.File.Move(v2, v1);
        }

        public DBEngine()
        {
            foreach (DbDataRecord r in OleDbEnumerator.GetRootEnumerator())
                Debug.WriteLine(string.Join(", ", 0.To(r.FieldCount - 1).Select((i) => r.GetValue(i))));
            Workspaces.Add(new CWorkSpace(this));
        }

    }
    public class CWorkSpace : IDBWorkSpace
    {
        private DBEngine dBEngine;
        private OleDbTransaction? _currentTransaction = default;

        public CWorkSpace(DBEngine dBEngine)
        {
            this.dBEngine = dBEngine;
        }

        public IList<IDatabase> Databases { get; } = [];

        public void Begin()
        {
            // Für OleDb gibt es keine explizite Transaktionsverwaltung auf Workspace-Ebene.
            // Stattdessen muss eine Transaktion auf der Connection gestartet werden.
            // Wir nehmen das erste geöffnete Database-Objekt, falls vorhanden.
            if (Databases.Count > 0 && Databases[0].Connection is OleDbConnection conn)
            {
                if (conn.State == ConnectionState.Open)
                {
                    // Beginnt eine Transaktion und speichert sie ggf. für Commit/Rollback
                    var transaction = conn.BeginTransaction();
                    // Optional: Transaktion referenzieren, falls später benötigt
                    this._currentTransaction = transaction;
                }
            }
            // Falls keine Datenbank offen ist, passiert nichts.
        }

        public void Commit()
        {
            // Für OleDb gibt es keine explizite Transaktionsverwaltung auf Workspace-Ebene.
            // Stattdessen muss eine Transaktion auf der Connection gestartet werden.
            // Wir nehmen das gemerkte Transaction-Objekt, falls vorhanden.
            _currentTransaction?.Commit();
            _currentTransaction = null;
        }

        public IDatabase CreateDatabase(string sDBName, string? sSQLChreate = null)
        {
            // Pseudocode:
            // 1. Erzeuge eine neue Access-Datenbankdatei mit ADOX.
            // 2. Nutze SQL DDL, um ggf. Initialstruktur zu erzeugen (optional).
            // 3. Öffne die neue Datenbank und gib ein IDatabase-Objekt zurück.

            // 1. Überprüfe, ob die Datei existiert und lösche sie ggf.
            if (System.IO.File.Exists(sDBName))
                System.IO.File.Delete(sDBName);

            // 2. Erstelle die Datenbank mit ADOX

            // 4. Öffne die neue Datenbank und gib das IDatabase-Objekt zurück
            var result = new OleDbConnection(new OleDbConnectionStringBuilder
                {
                    Provider = (IntPtr.Size == 8)?Settings.Default.OleDB_Provider64: Settings.Default.OleDB_Provider,
                    DataSource = sDBName
                }.ConnectionString);
            result.Open();
            return new CDatabase(result);
        }

        public IDatabase OpenDatabase(string path, bool v2, bool v3, string v4)
        {
            var result = new OleDbConnection() { ConnectionString = new OleDbConnectionStringBuilder() { Provider = Settings.Default.OleDB_Provider, DataSource = path, }.ConnectionString };
            result.Open();
            return new CDatabase(result);
        }

        public void Rollback()
        {
            // Für OleDb gibt es keine explizite Transaktionsverwaltung auf Workspace-Ebene.
            // Stattdessen muss eine Transaktion auf der Connection gestartet werden.
            // Wir nehmen das gemerkte Transaction-Objekt, falls vorhanden.
            _currentTransaction?.Rollback();
            _currentTransaction = null;
        }
    }

    public class CDatabase : IDatabase
    {
        public bool IsOpen => Connection != null && Connection.State == ConnectionState.Open;

        public CDatabase(DbConnection db)
        {
            Connection = db;
        }

        public DbConnection Connection { get; private set; }

        public void Close() => Connection.Close();

        public int Execute(string v, object? value = null)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = v;
            cmd.Prepare();
            if (value is IDictionary<string, object> dict)
            {
                foreach (var kvp in dict)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = kvp.Key;
                    parameter.Value = kvp.Value ?? DBNull.Value;
                    cmd.Parameters.Add(parameter);
                }
            }
            else if (cmd.Parameters.Count == 1 && value is object o)
                cmd.Parameters[0].Value = o;
           return cmd.ExecuteNonQuery();
        }

        public int Execute(string sql)
        {
            System.Diagnostics.Debug.WriteLine($"SQL:{sql}");
            var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            return cmd.ExecuteNonQuery();
        }

        public DataTable GetSchema(string v, params string[] strings)
        {
            return Connection.GetSchema(v, strings);
        }

        public IRecordset OpenRecordset(string v, object value, object missing, object missing1)
            => new Recordset(Connection, v, RecordsetTypeEnum.dbOpenTable);

        public IRecordset OpenRecordset(string v, RecordsetTypeEnum dbOpenTable = RecordsetTypeEnum.dbOpenQuery)
            => new Recordset(Connection, v, dbOpenTable);

        public IRecordset OpenRecordset(Enum eTable, RecordsetTypeEnum dbOpenTable = RecordsetTypeEnum.dbOpenTable)
        {
            return OpenRecordset(eTable.AsString(), dbOpenTable);
        }
    }

    public class Field : IField
    {
        public string Name => _idx.AsString();
        public object Value
        {
            get => _idx is int i ? _row[i] : _row[_idx.AsString()];
            set { if (_idx is int i) _row[i] = value; else _row[_idx.AsString()] = value; }
        }

        private DataRow _row;
        private object _idx;

        public int Size { get; internal set; }

        public Field(DataRow row, object idx)
        {
            _row = row;
            _idx = idx;
        }
    }
    public class FieldsCollection : IEnumerable, IFieldsCollection
    {
        private Recordset parent;
        // public string Name { get; }

        public IField this[string name] => new Field(parent.ActRow, name);
        public IField this[int idx] => new Field(parent.ActRow, idx);
        public IField this[Enum idx] => new Field(parent.ActRow, idx.AsString().TrimStart('_'));


        public IEnumerator GetEnumerator()
        {
            return parent.ActRow.ItemArray.GetEnumerator();
        }
        public FieldsCollection(Recordset parent)
        {
            this.parent = parent;
        }
    }
    public class Recordset : IRecordset
    {
        private DataTable _dataTable = new DataTable();
        private int dataRow;
        private string index;

        Dictionary<string, (IList<(string, int)>, string Sorting)> NamedIndex = new();
        private OleDbDataAdapter adapter;

        public DataRow ActRow => EditMode == 2 ? drNew : _dataTable.DefaultView[dataRow].Row;
        public Recordset(DbConnection db, string name, RecordsetTypeEnum type)
        {
            Name = name;
            _Type = type;
            Fields = new FieldsCollection(this);
            if (name.IsValidIdentifyer() && type == RecordsetTypeEnum.dbOpenTable)
                try
                {
                   var cmd = new OleDbCommand(name, (OleDbConnection)db);
                    cmd.CommandType = CommandType.TableDirect;
                    adapter = new OleDbDataAdapter(cmd);
                   adapter.Fill(_dataTable);
   //                adapter.
                   _dataTable.TableName = name;
                   var b = new OleDbCommandBuilder(adapter);
                   adapter.UpdateCommand = b.GetUpdateCommand();
                   adapter.InsertCommand = b.GetInsertCommand();
                   adapter.DeleteCommand = b.GetDeleteCommand();
                }
                catch {
                    _ = 1;
                }
            else
                try
                { _dataTable.Load(new OleDbCommand(name, (OleDbConnection)db).ExecuteReader()); }
                catch { }
            IsOpen = true;
            BuildIndex(db, name);
        }

        private void BuildIndex(DbConnection db, string name)
        {
            var Idx = db.GetSchema(OleDbMetaDataCollectionNames.Indexes);
            foreach (DataRow row in Idx.Rows)
                if (row[2].AsString().ToUpper() == name.ToUpper())
                {
                    var indexName = row[5].AsString();
                    if (!NamedIndex.ContainsKey(indexName.ToUpper()))
                        NamedIndex.Add(indexName.ToUpper(), (new List<(string, int)>() { (row[17].AsString(), row[16].AsInt()) }, ""));
                    else
                        NamedIndex[indexName.ToUpper()].Item1.Add((row[17].AsString(), row[16].AsInt()));
                }
            foreach (var key in NamedIndex.Keys.ToArray())
            {
                var idx = NamedIndex[key];
                var sort = string.Join(",", idx.Item1.OrderBy((i) => i.Item2).Select((i) => $"{i.Item1} ASC"));
                NamedIndex[key] = (idx.Item1, sort);
            }
        }

        public string Name { get; }

        public RecordsetTypeEnum _Type { get; }
        public string Index
        {
            get => index; set
            {
                if (index != value)
                    _dataTable.DefaultView.Sort = NamedIndex[value.ToUpper()].Sorting;
                index = value;
            }
        }
        public bool NoMatch => dataRow < 0;

        public IFieldsCollection Fields { get; }
        public int RecordCount => _dataTable.Rows.Count;
        public int EditMode { get; private set; }

        private DataRow? drNew = null;

        public bool EOF => dataRow >= _dataTable.Rows.Count;
        public bool BOF => dataRow < 0;

        public bool IsOpen { get; set; }

        public void Close()
        {
            // Setzt die DataTable und Zeiger zurück, um Ressourcen freizugeben
            _dataTable?.Dispose();
            _dataTable = new DataTable();
            IsOpen = false;
            dataRow = 0;
            index = null;
            NamedIndex.Clear();
        }

        public void Edit()
        {
            _dataTable.DefaultView[dataRow].Row.BeginEdit();
            if (EditMode == 0)
               EditMode = 1; // Setzt den Editiermodus
        }

        public void MoveNext()
        {
            _dataTable.RejectChanges();
            EditMode = 0; // Setzt den Editiermodus zurück
            if (!EOF)
                dataRow++;
        }

        public void Seek(string v, params object[] param)
        {
            // Unterstützt "=" und ">=" mit Index-Nutzung
            if (_dataTable.DefaultView.Sort == null || _dataTable.DefaultView.Sort == "")
            {
                // Kein Index gesetzt, kann nicht effizient suchen
                throw new InvalidOperationException("Kein Index gesetzt. Bitte Index-Eigenschaft setzen, bevor Seek verwendet wird.");
            }

            var sortColumns = _dataTable.DefaultView.Sort.Split(',')
                .Select(s => s.Trim().Split(' ')[0])
                .ToArray();

            if (param == null || param.Length == 0)
            {
                dataRow = -1;
                return;
            }
            DataView defaultView = _dataTable.DefaultView;

            // "=": exakte Suche mit Find
            if (v.Trim() == "=")
            {
                dataRow = defaultView.Find(param);
                    return;
            }
            // ">=": ersten passenden Eintrag suchen (optimiert, da DefaultView sortiert ist)
            else if (v.Trim() == ">=")
            {
                var comparer = StringComparer.InvariantCultureIgnoreCase;
                // Ersetze die lineare Suche durch eine Binärsuche
                int left = 0;
                int right = defaultView.Count - 1;
                int foundIndex = -1;
                while (left <= right)
                {
                    int mid = left + (right - left) / 2;
                    int cmp = CompareRow(param, sortColumns, defaultView[mid]);
                    if (cmp == 0)
                    {
                        foundIndex = mid;
                        break;
                    }
                    else if (cmp < 0)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid - 1;
                    }
                }
                if (foundIndex >= 0)
                {
                    dataRow = foundIndex;
                }
                else if (left < defaultView.Count)
                {
                    dataRow = left;
                }
                else
                    dataRow = -1;
            }
            else
            {
                throw new NotSupportedException($"Seek operation '{v}' is not supported.");
            }

            static int CompareRow(object[] param, string[] sortColumns, DataRowView rowView)
            {
                var cmp = 0;
                for (int c = 0; c < param.Length && c < sortColumns.Length; c++)
                {
                    var colValue = rowView[sortColumns[c]];
                    if (DBNull.Value == colValue)
                        return 1;
                    cmp = Comparer.Default.Compare( colValue, param[c]);
                    if (cmp < 0 || cmp > 0)
                    {
                        break;
                    }
                }
                if (cmp == 0 && param.Length < sortColumns.Length)
                    cmp = 1;
                return cmp;
            }
        }

        public void Update()
        {
            if (EditMode == 2)
            {    
                _dataTable.Rows.Add(drNew);
                dataRow = _dataTable.Rows.Count - 1;
                drNew?.EndEdit();
                drNew = null;
            }
            else
                _dataTable.DefaultView[dataRow].Row.EndEdit();
            adapter?.Update(_dataTable);
            _dataTable.AcceptChanges();
            EditMode = 0; // Setzt den Editiermodus zurück
        }

        public void AddNew()
        {
            EditMode = 2; // Setzt den Appendmodus
            drNew = _dataTable.NewRow();
            drNew.BeginEdit();
            dataRow = _dataTable.Rows.Count; // Setzt den Zeiger auf die neue Zeile
        }

        public void Delete()
        {
            _dataTable.DefaultView[dataRow].Row.Delete();
            adapter?.Update(_dataTable);
        }

        public void MoveLast()
        {
            _dataTable.RejectChanges();
            dataRow = _dataTable.Rows.Count - 1;
        }

        public void FindFirst(string sWhereExpr)
        {
            // Erwartet: "<FeldName> = <Wert>"
            if (string.IsNullOrWhiteSpace(sWhereExpr) || _dataTable.Rows.Count == 0)
                return;

            var parts = sWhereExpr.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return;

            var field = parts[0].Trim();
            var valueStr = parts[1].Trim().Trim('\'');

            // Versuche, den Wert passend zum Spaltentyp zu parsen
            object value = valueStr;
            if (_dataTable.Columns.Contains(field))
            {
                var colType = _dataTable.Columns[field].DataType;
                try
                {
                    if (colType == typeof(int))
                        value = int.Parse(valueStr);
                    else if (colType == typeof(long))
                        value = long.Parse(valueStr);
                    else if (colType == typeof(double))
                        value = double.Parse(valueStr);
                    else if (colType == typeof(DateTime))
                        value = DateTime.Parse(valueStr);
                    else if (colType == typeof(bool))
                        value = bool.Parse(valueStr);
                    else
                        value = valueStr;
                }
                catch
                {
                    value = valueStr;
                }

                for (int i = 0; i < _dataTable.Rows.Count; i++)
                {
                    var rowValue = _dataTable.Rows[i][field];
                    if (rowValue != DBNull.Value && rowValue.Equals(value))
                    {
                        dataRow = i;
                        return;
                    }
                }
            }
            // Kein Treffer: Zeiger auf "NoMatch"
            dataRow = -1;
        }

        public void MoveFirst()
        {
            _dataTable.RejectChanges();
            dataRow = 0;
        }

        public void MovePrevious()
        {
            if (!BOF)
                dataRow--;
        }
    }

    public static IServiceCollection AddOleDB(this IServiceCollection services)
    {
        services.AddSingleton<IDBEngine, DBEngine>();
        services.AddSingleton<IDBWorkSpace, CWorkSpace>();
        services.AddTransient<IDatabase, CDatabase>();
        services.AddTransient<IRecordset, Recordset>();
        services.AddTransient<IFieldsCollection, FieldsCollection>();
        services.AddTransient<IField, Field>();
        return services;
    }
}
