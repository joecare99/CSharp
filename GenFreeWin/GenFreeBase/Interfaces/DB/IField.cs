//using DAO;
using BaseLib.Interfaces;
using GenFree.Interfaces.Model;

namespace GenFree.Interfaces.DB;

public interface IField: IHasValue
{
    string Name { get; }
    int Size { get; }
    new object Value { get; set; }
}