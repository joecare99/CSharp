//using DAO;
namespace GenFree.Interfaces.Model
{
    public interface IHasID<T>

    {
        T MaxID { get; }
        void Delete(T key);
        bool Exists(T key);
    }
}