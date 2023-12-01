//using DAO;
namespace GenFree.Interfaces.Model
{
    public interface IUsesID<T>

    {
        T MaxID { get; }
        void Delete(T key);
        bool Exists(T key);
    }
}