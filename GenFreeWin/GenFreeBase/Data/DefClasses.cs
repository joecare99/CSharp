﻿//using DAO;
using System.Data.Common;
using System.Collections.Generic;
using System;
using System.Linq;
using GenFree.Interfaces.DB;

namespace GenFree.Data;

public class TableDef : ITableDef
{
    public TableDef(DbConnection db, string v)
        => (Name, _db) = (v, db);

    protected DbConnection _db;
    public string? Name { get; set; }
    public IList<IFieldDef> Fields { get; init; } = [];
    public IList<IIndexDef> Indexes { get; init; } = [];
}

public class IndexDef : IIndexDef
{
    public IndexDef(ITableDef td, string name, string sField, bool xPrimary, bool xUnique)
    {
        _table = td;
        IIndexDef? ix;
        if ((ix = td?.Indexes.FirstOrDefault(i => i.Name == name)) != null)
        {
            var Fld = (ix.Fields ?? []).ToList();
            Fld.Add(sField);
            ix.Fields = Fields = Fld.ToArray();
        }
        else
        {
            Name = name;
            Fields = new string[] { sField };
            Primary = xPrimary;
            Unique = xUnique;
            if (td != null)
               td.Indexes.Add(this);
        }
    }
    private ITableDef _table;

    public string? Name { get; set; }
    public string[]? Fields { get; set; } = default;
    public bool Primary { get; set; } = false;
    public bool Unique { get; set; } = false;
    public bool IgnoreNulls { get; set; } = false;
}

public class FieldDef : IFieldDef
{
    public FieldDef(ITableDef td, string name, Enum v2, int v3)
    {
        _table = td;
        Name = name;
        Type = (TypeCode)v2;
        Size = v3;
        if(td!=null)
          td.Fields.Add(this);
    }

    private ITableDef _table;
    public string? Name { get; set; }
    public TypeCode Type { get; set; } = default;
    public int Size { get; set; } = -1;
    public bool xNull { get; set; } = false;
    public bool Required { get; private set; } = false;
}


