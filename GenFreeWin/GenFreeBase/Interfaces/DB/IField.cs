//using DAO;
using BaseLib.Interfaces;

namespace GenFree.Interfaces.DB;

public interface IField : IHasValue
{
    string Name { get; }
    int Size { get; }
    new object Value { get; set; }
}