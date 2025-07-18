﻿using BaseLib.Helper;
using GenFree.Data;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GenFree.Helper;
public static class DBHelper
{
    public static IEnumerable<ITableDef> TableDefs(this IDatabase db)
    {
        var dbDataTypes = db.GetDataTypes();
        var enumerator = db.GetSchema("Tables").Rows;
        foreach (DataRow row in enumerator)
        {
            TableDef td = new(db.Connection, row["TABLE_NAME"].AsString());
            db.GetSchema("Columns", new[] { null, null, td.Name }).Rows
                .Cast<DataRow>().Select(
                r => new FieldDef(td, r["COLUMN_NAME"].AsString(),dbDataTypes.GetTypeCode(r["DATA_TYPE"].AsInt()), r["CHARACTER_MAXIMUM_LENGTH"].AsInt())).ToList().ForEach(r => { });
            db.GetSchema("Indexes").Rows
                .Cast<DataRow>().Where(r => r["TABLE_NAME"].AsString() == td.Name)?.Select(
                r => new IndexDef(td, r["INDEX_NAME"].AsString(), r["COLUMN_NAME"].AsString(), r["PRIMARY_KEY"].AsBool(), r["UNIQUE"].AsBool())).ToList().ForEach(r => { /*if (r.Name == null) td.Indexes.Add(r);*/ });
            yield return td;
        }
    }

    public static IDictionary<int,(string name,TypeCode tc)> GetDataTypes(this IDatabase db)
    {
        Dictionary<int, (string name, TypeCode tc)> result = [];
        var datatypes = db.GetSchema("DataTypes");
        result[130] = ("VarChar", TypeCode.String);
        // 11
        foreach (DataRow datatype in datatypes.Rows)
        {
            result[datatype[1].AsInt()]=(datatype[0].AsString(),Type.GetTypeCode(Type.GetType(datatype[5].AsString(),false,true)));
        }
        return result;
    }

    public static TypeCode GetTypeCode(this IDictionary<int, (string name, TypeCode tc)> dbTypes, int iType)
    {
        if (dbTypes.TryGetValue(iType, out var rslt))
            return rslt.tc;
        else
            return TypeCode.Empty;
    }

    public static bool DbFieldExists(this IDatabase TabDef, Enum field, string sFieldName)
    {
        try
        {
            var enumerator = TabDef.GetSchema("Columns").Rows;
            foreach (DataRow row in enumerator)
            {
                if (row["TABLE_NAME"].AsString().ToLower() == field.AsString().ToLower())
                    if (row["COLUMN_NAME"].AsString().ToLower() == sFieldName.ToLower())
                    {
                        return true;
                    }
            }
            bool result = default;
            return result;
        }
        catch
        {
        }
        return false;
    }
    public static bool TableExists(this IDatabase db, string mytable)
    {
        try
        {
            foreach (DataRow row in db.GetSchema("Tables").Rows)

                if (row["TABLE_NAME"].AsString().ToLower() == mytable.AsString().ToLower())
                {
                    return true;
                }
        }
        catch
        {
        }
        return false;
    }

    public static void TryExecute(this IDatabase db, string sql, object? val)
    {
        try
        {
            db.Execute(sql, val);
        }
        catch
        {
        }
    }
    public static void TryExecute(this IDatabase db, string sql)
    {
        try
        {
            db.Execute(sql);
        }
        catch
        {
        }
    }

    public static void CreateField(this IDatabase db, ITableDef tbl, IFieldDef val)
    {
        try
        {
            db.Execute($"ALTER TABLE {tbl.Name} ADD {val.Name} {val.Type.ToDBType(val.Size)}");
        }
        catch
        {
        }
    }

    public static void CreateIndex(this IDatabase db, ITableDef tbl, IIndexDef idx)
    {
        try
        {
            db.Execute($"CREATE {(idx.Unique ? "UNIQUE " : "")}INDEX [{idx.Name}] ON [{tbl.Name}] ({string.Join(",", idx.Fields.Select(s => $"[{s}]"))}) {(idx.Primary ? "WITH PRIMARY" : "")};");
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine($"Error creating index {idx.Name} on table {tbl.Name}: {string.Join(",", idx.Fields)}");
        }
    }

    public static void AlterIndex(this IDatabase db, ITableDef tbl, IIndexDef idx)
    {
      //  var tr=db.Connection.BeginTransaction();
        try
        {
            // Drop the existing index if it exists
            db.TryExecute($"DROP INDEX [{idx.Name}] ON [{tbl.Name}]");

            // Create the new index with updated properties
            db.Execute($"CREATE {(idx.Unique ? "UNIQUE " : "")}INDEX [{idx.Name}] ON [{tbl.Name}] ({string.Join(",", idx.Fields.Select(s => $"[{s}]"))}) {(idx.Primary ? "WITH PRIMARY" : "")};");
            //tr.Commit();
        }
        catch (Exception ex)
        {
          //  tr.Rollback();
            System.Diagnostics.Debug.WriteLine($"Error creating index {idx.Name} on table {tbl.Name}: {string.Join(",", idx.Fields)}");
        }
    }

    public static void CreateTable(this IDatabase db, ITableDef tbl )
    {
        try
        {
            db.Execute($"CREATE TABLE {tbl.Name} ({string.Join(",", tbl.Fields.Select(f => $"{f.Name} {f.Type.ToDBType(f.Size)}"))});");
            foreach (var idx in tbl.Indexes)
            {
                db.CreateIndex(tbl, idx);
            }
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine($"Error creating table {tbl.Name}: {string.Join(",", tbl.Fields)}");
        }
    }

    public static string ToDBType(this TypeCode type, int size = 0)
    {
        return type switch
        {
            TypeCode.Boolean => "BIT",
            TypeCode.Byte => "TINYINT",
            TypeCode.Char => "CHAR",
            TypeCode.DateTime => "DATETIME",
            TypeCode.Decimal => "DECIMAL",
            TypeCode.Double => "DOUBLE",
            TypeCode.Int16 => "SMALLINT",
            TypeCode.Int32 => "INTEGER",
            TypeCode.Int64 => "BIGINT",
            TypeCode.SByte => "TINYINT",
            TypeCode.Single => "FLOAT",
            TypeCode.String when size>0 => $"VARCHAR({size})",
            TypeCode.String when size==0 => "MEMO",
            _ => throw new ArgumentException($"Unsupported type: {type}"),
        };
    }

    /// <summary>
    /// Begins the transaction.
    /// </summary>
    /// <param name="wks">The WKS.</param>
    public static void BeginTrans(this IDBWorkSpace wks) => wks.Begin();
    /// <summary>
    /// Commits the transaction.
    /// </summary>
    /// <param name="wks">The WKS.</param>
    public static void CommitTrans(this IDBWorkSpace wks) => wks.Commit();

    /// <summary>
    /// Converts an Enum as a fieldname.
    /// </summary>
    /// <param name="eFld">The field as enum.</param>
    /// <returns>System.String.</returns>
    public static string AsFld(this Enum eFld) => $"{eFld}".TrimStart('_');

    /// <summary>
    /// This does the Action fo reach record in the recordset.
    /// </summary>
    /// <param name="recordset">The table.</param>
    /// <param name="doActn">The action to do.</param>
    public static void ForEachDo(this IRecordset recordset, Action<IFieldsCollection> doActn)
    {
        recordset.MoveFirst();
        while (!recordset.EOF)
        {
            recordset.Edit();
            doActn(recordset.Fields);
            recordset.Update();
            recordset.MoveNext();
        }
    }

}
