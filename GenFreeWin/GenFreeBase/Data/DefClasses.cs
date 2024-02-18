//using DAO;
using System.Data.Common;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GenFree.Data;

public class TableDef
{
    public TableDef(DbConnection db, string v)
    {
        Name = v;
    }
    private DbConnection _db;
    public string? Name { get; set; }
    public List<FieldDef> Fields { get; } = new();
    public List<IndexDef> Indexes { get; } = new();
}

public class IndexDef
{
    public IndexDef(TableDef td, string name, string sField, bool xPrimary,bool xUnique)
    {
        _table = td;
        IndexDef? ix;
        if ((ix = td.Indexes.Find(i => i.Name==name)) != null)
        {
            var Fld = ix.Fields.ToList();
            Fld.Add(sField);
            ix.Fields = Fld.ToArray();
        }
        else
        {
            Name = name;
            Fields = new string[] { sField };
            Unique = xUnique;
            td.Indexes.Add(this);
        }
    }
    private TableDef _table;

    public string? Name { get; set; }
    public string[]? Fields { get; set; } = default;
    public bool Unique { get; set; } = false;
    public bool IgnoreNulls { get; set; } = false;
}

public class FieldDef
{
    public FieldDef(TableDef td, string name, string v2, int v3)
    {
        _table = td;
        Name = name;
        Type = (TypeCode)Enum.Parse(typeof(TypeCode), v2, true);
        Size = v3;
        td.Fields.Add(this);
    }

    private TableDef _table;
    public string? Name { get; set; }
    public TypeCode Type { get; set; } = default;
    public int Size { get; set; } = -1;
    public bool xNull { get; set; } = false;
    public bool Required { get; private set; } = false;
}


