//using DAO;
using GenFree.Data;
using System.Collections.Generic;

namespace GenFree.Interfaces.DB
{
    public interface ITableDef
    {
        List<IFieldDef> Fields { get; }
        List<IIndexDef> Indexes { get; }
        string? Name { get; set; }
    }
}