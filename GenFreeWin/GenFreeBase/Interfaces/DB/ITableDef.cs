//using DAO;
using GenFree.Data;
using System.Collections.Generic;

namespace GenFree.Interfaces.DB
{
    public interface ITableDef
    {
        IList<IFieldDef> Fields { get; }
        IList<IIndexDef> Indexes { get; }
        string? Name { get; set; }
    }
}