//using DAO;
using System;

namespace GenFree.Interfaces.DB
{
    public interface IFieldDef
    {
        string? Name { get; set; }
        bool Required { get; }
        int Size { get; set; }
        TypeCode Type { get; set; }
        bool xNull { get; set; }
    }
}