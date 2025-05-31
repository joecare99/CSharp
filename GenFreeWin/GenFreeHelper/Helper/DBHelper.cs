﻿using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GenFree.Interfaces.DB;
using BaseLib.Helper;

namespace GenFree.Helper;
public static class DBHelper
{
    public static IEnumerable<TableDef> TableDefs(this IDatabase db)
    {
        var enumerator = db.GetSchema("Tables").Rows;
        foreach (DataRow row in enumerator)
        {
            TableDef td = new(db.Connection, row["TABLE_NAME"].AsString());
            db.GetSchema("Columns", new[] { null, null, td.Name }).Rows.Cast<DataRow>().Select(
                r => new FieldDef(td, r["COLUMN_NAME"].AsString(), r["DATA_TYPE"].AsString(), r["CHARACTER_MAXIMUM_LENGTH"].AsInt())).ToList().ForEach(r => { });
            db.GetSchema("Indexes").Rows.Cast<DataRow>().Where(r => r["TABLE_NAME"].AsString() == td.Name).Select(
                r => new IndexDef(td, r["INDEX_NAME"].AsString(), r["COLUMN_NAME"].AsString(), r["PRIMARY_KEY"].AsBool(), r["UNIQUE"].AsBool())).ToList().ForEach(r => { /*if (r.Name == null) td.Indexes.Add(r);*/ });
            yield return td;
        }
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

    public static void TryExecute(this IDatabase db, string sql, object? val = null)
    {
        try
        {
            db.Execute(sql, val);
        }
        catch
        {
        }
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
