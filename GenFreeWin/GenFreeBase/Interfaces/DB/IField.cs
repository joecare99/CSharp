//using DAO;
namespace GenFree.Interfaces.DB;

public interface IField
{
    string Name { get; }
    int Size { get; }
    object Value { get; set; }
}