//using DAO;
namespace GenFree.Interfaces.DB
{
    public interface IIndexDef
    {
        string[]? Fields { get; set; }
        bool IgnoreNulls { get; set; }
        string? Name { get; set; }
        bool Unique { get; set; }
    }
}