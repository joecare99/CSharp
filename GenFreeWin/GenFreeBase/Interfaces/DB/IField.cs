//using DAO;
using GenFree.Interfaces.Model;

namespace GenFree.Interfaces.DB;

public interface IField: IHasValue
{
    string Name { get; }
    int Size { get; }
    object? Value { get; set; }
}